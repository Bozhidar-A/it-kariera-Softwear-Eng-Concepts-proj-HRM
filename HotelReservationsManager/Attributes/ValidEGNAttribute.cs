using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManager.Attributes
{
    public class ValidEGNAttribute : ValidationAttribute
    {
        public ValidEGNAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string EGN = (string)value;

            if (EGN.Length != 10)
            {
                return new ValidationResult($"This is not a valid EGN");
            }
            foreach (char digit in EGN)
            {
                if (!Char.IsDigit(digit))
                {
                    return new ValidationResult($"This is not a valid EGN");
                }
            }
            int month = int.Parse(EGN.Substring(2, 2));
            int year = 0;
            if (month < 13)
            {
                year = int.Parse("19" + EGN.Substring(0, 2));
            }
            else if (month < 33)
            {
                month -= 20;
                year = int.Parse("18" + EGN.Substring(0, 2));
            }
            else
            {
                month -= 40;
                year = int.Parse("20" + EGN.Substring(0, 2));
            }
            int day = int.Parse(EGN.Substring(4, 2));
            //DateTime dateOfBirth = new DateTime();
            //if (!DateTime.TryParse(String.Format("{0}/{1}/{2}", day, month, year), out dateOfBirth))
            //{
            //    return new ValidationResult($"This is not a valid EGN");
            //}
            int[] weights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int totalControlSum = 0;
            for (int i = 0; i < 9; i++)
            {
                totalControlSum += weights[i] * (EGN[i] - '0');
            }
            int controlDigit = 0;
            int reminder = totalControlSum % 11;
            if (reminder < 10)
            {
                controlDigit = reminder;
            }
            int lastDigitFromIDNumber = int.Parse(EGN.Substring(9));
            if (lastDigitFromIDNumber != controlDigit)
            {
                return new ValidationResult($"This is not a valid EGN");
            }
            return ValidationResult.Success;
        }
    }
}
