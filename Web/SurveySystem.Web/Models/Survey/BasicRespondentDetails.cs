namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;

    public class BasicRespondentDetails
    {
        public BasicRespondentDetails(
            string firstName,
            string lastName,
            string email,
            string facultyNumber,
            string ip)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.FacultyNumber = facultyNumber;
            this.IP = ip;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string FacultyNumber { get; set; }

        public string IP { get; set; }
    }
}