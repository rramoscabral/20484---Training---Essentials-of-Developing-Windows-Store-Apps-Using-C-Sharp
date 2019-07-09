using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNETCoreRESTFulWS.Models
{
    public class PearsonRegistration
    {
        List<Pearson> PearsonList;

        static PearsonRegistration pearsonRegistry = null;

        private PearsonRegistration()
        {
            PearsonList = new List<Pearson>();
            InicializarDados();
        }

        public static PearsonRegistration getInstance()
        {
            if (pearsonRegistry == null)
            {
                pearsonRegistry = new PearsonRegistration();
                return pearsonRegistry;
            }
            else
            {
                return pearsonRegistry;
            }
        }

        public void Add(Pearson Pearson)
        {
            PearsonList.Add(Pearson);
        }

        public String Remove(int ID)

        {
            for (int i = 0; i < PearsonList.Count; i++)
            {
                Pearson stdn = PearsonList.ElementAt(i);
                if (stdn.ID == ID)
                {
                    PearsonList.RemoveAt(i);//update the new record
                    return "Apagado com sucesso";
                }
            }
            return "Sem sucesso ";
        }

        public List<Pearson> getAllPearson()
        {
            return PearsonList;
        }

        public String UpdatePearson(Pearson std)
        {
            for (int i = 0; i < PearsonList.Count; i++)
            {
                Pearson stdn = PearsonList.ElementAt(i);
                if (stdn.ID == std.ID)
                {
                    // Atualiza o registo
                    PearsonList[i] = std;
                    return "Atualizado com sucesso";
                }
            }
            return "Atualização sem sucesso";
        }

        private void InicializarDados()
        {
            PearsonList = new List<Pearson>()
            {
                new Pearson() { ID =  01, Name = "Anya Silva", Email = "asilva@asilva.com" },
                new Pearson() { ID =  02, Name = "Zen Lucas", Email = "luen@topluen.pt" },
                new Pearson() { ID =  03, Name = "Mara Ribas", Email = "mabas@mabas" },
            };
        }

        public int NextID
        {
            get { 
            var lastID = (from listLastID in PearsonList
                          select listLastID.ID).LastOrDefault();
            return lastID + 1;
           }
        }
    }
}
