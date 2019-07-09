using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNETCoreRESTFulWS.Models
{
    public class PearsonRegistrationReply
    {

        int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }


        String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }



        String _email;

        public String Email
        {
            get { return _email; }
            set { _email = value; }
        }
        String registrationStatus;

        public String RegistrationStatus
        {
            get { return registrationStatus; }
            set { registrationStatus = value; }
        }

    }
}
