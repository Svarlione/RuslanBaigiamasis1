using System.ComponentModel.DataAnnotations;
//using RuslanAPI.DataLayer.Data;//ne eina prijungti prie bazes


namespace RuslanAPI.Core.Attributes
{
    public class UniqueUserNameAttribute : ValidationAttribute
    {
        //patikrina ar domenu bazeje nepasikartoja user name(unikalus)
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //if (value != null)
            //{
            //    var userName = value.ToString();
            //    var dbContext = (UserDbContext)validationContext.GetService(typeof(UserDbContext));

            //    // Patikriname, ar vartotojo vardas jau yra duomenų bazėje
            //    if (dbContext.Users.Any(u => u.UserName == userName))
            //    {
            //        return new ValidationResult("Šis vartotojo vardas jau yra naudojamas.");
            //    }
            //}

            return ValidationResult.Success;
        }
    }

}
