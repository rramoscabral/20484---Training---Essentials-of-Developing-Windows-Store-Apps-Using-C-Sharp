using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNETCoreRESTFulWS.Models
{
    /// <summary>
    /// Classe Pessoa
    /// </summary>
    public class Pearson
    {
        private int _id;
        private string _name;
        private string _email;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Construtor por defeito
        /// </summary>
        public Pearson()
        {
           //ID = PearsonRegistration.getInstance().NextID;
           ID = 0;
           Name = String.Empty;
           Email = String.Empty;
        }

        public Pearson(int iD, string name, string email)
        {
            ID = iD;
            this.Name = name;
            Email = email;
        }


        /// <summary>
        /// Detalhes da pessoa
        /// </summary>
        /// <returns>Retorna a identificação, nome e correio electrónico</returns>
        public string Details()
        {
            return String.Format( $"ID: {_id} - Name: {_name}  - correio electrónico: {_email}");
        }


    }
}
