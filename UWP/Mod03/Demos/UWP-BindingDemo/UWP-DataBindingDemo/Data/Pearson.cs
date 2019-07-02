using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DataBindingDemo.Data
{
    public class Pearson
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Pearson()
        {
            this.ID = 1;
            this.Name = "Anya Silva";
            this.Email = "asilva@anya-silva.pt";
        }
        public string Details
        {
            get
            {
                return $"{this.ID} - {this.Name}, correio electrónico: "
                    + this.Email;
            }
        }
    }

    public class PearsonViewModel
    {
        private Pearson defaultPearson = new Pearson();
        public Pearson DefaultPearson => this.defaultPearson;
    }


    public class PearsonViewModelCollection
    {

        private ObservableCollection<Pearson> pearsons = new ObservableCollection<Pearson>();

        public ObservableCollection<Pearson> Pearsons { get { return this.pearsons; } }

        public PearsonViewModelCollection()
        {
            this.Pearsons.Add(new Pearson()
            {
                ID = 1,
                Name = "Zen Carmo",
                Email = "zcarmo@xpto.pt"
            });
            this.Pearsons.Add(new Pearson()
            {
                ID = 2,
                Name = "Filipa Marques",
                Email = "fm@fm.pt"
            });
        }
    }
}

