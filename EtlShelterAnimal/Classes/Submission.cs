using System;

namespace EtlShelterAnimal.Classes
{
    public class Submission
    {
        public Submission(int id, string outcome)
        {
            ID = id;

            if (outcome == "Adoption") Adoption = 1;
            else if (outcome == "Died") Died = 1;
            else if (outcome == "Euthanasia") Euthanasia = 1;
            else if (outcome == "Return_to_owner") Return_to_owner = 1;
            else if (outcome == "Transfer") Transfer = 1;
            else throw new Exception("Outcome type invalid");
        }

        public int ID { get; set; }

        public int Adoption { get; set; }

        public int Died { get; set; }

        public int Euthanasia { get; set; }

        public int Return_to_owner { get; set; }

        public int Transfer { get; set; }
    }
}
