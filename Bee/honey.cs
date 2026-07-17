using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beehive
{

    static class HoneyVault
    {
        private const float NECTAR_CONVERTION_RATIO = 0.19f;
        private const float LOW_LEVEL_WARNING = 10.0f;

        private static float honey = 25f;
        private static float nectar = 100f;

        public static void CollectNectar(float amount)
        {
            if (amount > 0f) nectar += amount;
        }

        public static void ConvertNectarToHoney(float amount)
        {
            float nectarToConvert = amount;
            if (nectarToConvert > nectar) nectarToConvert = nectar;
            nectar -= nectarToConvert;
            honey += nectarToConvert * NECTAR_CONVERTION_RATIO;
        }

        public static bool ConsumeHoney(float amount)
        {
            if (amount <= honey)
            {
                honey -= amount;
                return true;
            }
            else
            {
                Console.Write("No honey enough baby");
                return false;
            }
        }

        public static string StatusReport
        {
            get
            {
                string status = $"{honey:0.0} units of honey\n" +
                                $"{nectar:0.0} units of nectar";
                string warnings = "";
                if (honey < LOW_LEVEL_WARNING)
                    warnings += "\nLow honey - add a honey manufacturer";
                if (nectar < LOW_LEVEL_WARNING)
                    warnings += "\nLow nectar - add a nectar collector";
                return status + warnings;
            }
        }

    }

    public class Bee
    {
        public virtual float CostPerShift { get; }

        public string Job { get; private set; }
        public Bee(string job)
        {
            Job = job;
        }

        public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
            {
                DoJob();
            }
        }

        protected virtual void DoJob()
        {
            /*Métodos implementados em cada sub-classe*/
        }
    }

    public class Queen : Bee
    {
        private Bee[] workers = new Bee[0];
        private float eggs = 0;
        private float unassignedWorker = 3;
        public float UnassignedWorkers => unassignedWorker;
        private const float EGGS_PER_SHIFT = 0.45f;
        public const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

        public string StatusReport { get; private set; }
        public Queen() : base("Queen")
        {
            AssignBee("Egg Care");
            AssignBee("Nectar Collector");
            AssignBee("Honey Manufacturer");
        }
        public override float CostPerShift
        {
            get { return 2.15f; }
        }

        private void AddWorker(Bee worker)
        {
            if(unassignedWorker >= 1)
            {
                unassignedWorker--;
                Array.Resize(ref workers, workers.Length + 1);
                workers[workers.Length - 1] = worker;
            }
        }

        public void AssignBee(string job)
        {
            switch (job) 
            {
                case "Egg Care":
                    AddWorker(new EggCare(this));
                    break;
                case "Nectar Collector":
                    AddWorker(new NectarCollector());
                    break;
                case "Honey Manufacturer":
                    AddWorker(new HoneyManufecturer());
                    break;
                default:
                    return;
            }
        }

        protected override void DoJob()
        {
            eggs = eggs + EGGS_PER_SHIFT;
            foreach(Bee worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(unassignedWorker * HONEY_PER_UNASSIGNED_WORKER);
            UpdateStatusReport();
        }

        public void CareForEggs(float eggsToConvert)
        {
            if(eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorker += eggsToConvert;
            }
            else
            {
                return;
            }
        }

        public void UpdateStatusReport()
        {
            StatusReport = $"Vault report:\n{HoneyVault.StatusReport}\n" +
                           $"\nEgg count: {eggs:0.0}\nUnassigned workers: {unassignedWorker:0.0}\n" +
                           $"{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Honey Manufacturer")}" +
                           $"\n{WorkerStatus("Egg Care")}\nTOTAL WORKERS: {workers.Length}";
        }

        private string WorkerStatus(string job)
        {
            int count = 0;
            foreach (Bee worker in workers)
            {
                if (worker.Job == job) count++;
            }
            string s = "s";
            if (count == 1) s = "";
            return $"{count} {job} bee{s}";
        }
    }

    class NectarCollector : Bee 
    {
        public const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;
        public NectarCollector() : base("Nectar Collector")
        {

        }

        public override float CostPerShift
        {
            get { return 1.95f; }
        }

        protected override void DoJob()
        {
            HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
        }

    }

    class HoneyManufecturer : Bee
    {
        public const float NECTAR_PROCESSED_PER_SHIFT = 33.15f;
        public HoneyManufecturer() : base("Honey Manufacturer")
        {

        }
        public override float CostPerShift
        {
            get { return 1.7f; }
        }

        protected override void DoJob()
        {
            HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSED_PER_SHIFT);
        }

    }

    class EggCare : Bee
    {
        public const float CARE_PROGRESS_PER_SHIFT = 0.15f;

        private Queen queen;

        public override float CostPerShift
        {
            get { return 1.35f; }
        }

        public EggCare(Queen queen) : base("Egg Care")
        {
            this.queen = queen;
        }

        protected override void DoJob()
        {
            queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
        }
    }
}
