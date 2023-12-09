using FluentValidation;
using FluentValidation.Results;
using Performance.Base;

namespace Performance.FluentValidaton
{
    public class Customer
    {
        public string Surname { get; set; }
        public string Forename { get; set; }
        public static void SafeAdd<T>(ref List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>(1);
            }

            list.Add(item);
        }

        public IReadOnlyList<string> Validate(Customer customer)
        {
            List<string> results = null;

            if (customer.Surname == null)
            {
                SafeAdd(ref results, "Surname is empty");
            }

            if (customer.Forename == null)
            {
                SafeAdd(ref results, "Please specify a first name");
            }

            return results;
        }

        public static void Test()
        {
            var validator = new CustomerValidator();

            PerfTest.MeasurePerf(
            () =>
            {
                var customer = new Customer();

                customer.Surname = "Last";
                customer.Forename = "First";

                ValidationResult results = validator.Validate(customer);
                bool success = results.IsValid;

            },
            "FluentValidation",
            1000 * 1000);


            PerfTest.MeasurePerf(
            () =>
            {
                var customer = new Customer()
                {
                    Surname = "Last",
                    Forename = "First"
                };

                var results = customer.Validate(customer);
                bool sucess = results is null;

            },
            "Custom",
            1000 * 1000);

        }
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.Surname).NotEmpty();
            RuleFor(customer => customer.Forename).NotEmpty();
        }

    }
}


