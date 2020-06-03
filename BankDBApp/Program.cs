using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BankDBApp
{
    public static class BankDefs
    {
        public const int BankAccount = 1;
        public const int CreditAccount = 2;
    }

    class Program
    {
        public static PankkiEntities context; 

        static void Main(string[] args)
        {
            context = new PankkiEntities();

            // App title

            Console.WriteLine("BANK");
            Console.WriteLine("====");
            bool leaveBank = default;
            do
            {
                switch (GUIMainDisplay())
                {
                    case 0:
                        leaveBank = true;
                        Console.WriteLine("Leaving Bank...");
                        break;
                    case 1:
                        GUICreateCustomer();
                        break;
                    case 2:
                        GUICreateBankAccount();
                        break;
                    case 3:
                        GUICreateCreditAccount();
                        break;
                    case 4:
                        GUIMoveAccountToNewCustomer();
                        break;
                    case 6:
                        GUIShowCustomer();
                        break;
                    case 7:
                        GUIShowAccount();
                        break;
                    case 8:
                        GUIDeleteCustomer();
                        break;
                    case 9:
                        GUIDeleteAccount();
                        break;
                    case 10:
                        GUIChangeSaldo();
                        break;
                    default:
                        break;
                }

            } while (!leaveBank);


            // end program
            Console.ReadLine();
        }


        private static int GUIMainDisplay()
        {
            bool validResponse = false;
            int response;
            do
            {
                Console.WriteLine(@"
                        Select Activity (0 to 10)
                        0) Lopetus
                        1) Uusi Asiakas
                        2) Uusi Pankkitili
                        3) Uusi Luottotili
                        4) Siirrä tili toiselle asiakkaalle
                        6) Näytä asiakkaat
                        7) Näytä tilit
                        8) Poista asiakas
                        9) Poista tili
                        10) Tee tilitapahtumia (nosto ja talletus)
                    ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response);
            } while (!validResponse);

            return response;
        }

        private static void GUICreateCustomer()
        {
            Console.WriteLine(@"
                        Customer First Name?
        ");
            string firstInput = Console.ReadLine();
            
            Console.WriteLine(@"
                            Customer Family Name?
            ");
            string familyInput = Console.ReadLine();

            if (confirmInput())
            {
                var newCustomer = new customers()
                {
                    customer_first_name = firstInput,
                    customer_last_name = familyInput,
                };
                context.customers.Add(newCustomer);
                context.SaveChanges();
            }
        }

        private static void GUICreateBankAccount()
        {
            bool validResponse = false;

            int customerNumber = default;
            decimal creditLimit = default;
            decimal currentSaldo = default;

            GUIShowCustomer();

            do
            {
                Console.WriteLine(@"
                        Who gets new account (customer number)?
                ");
                string guessInput = Console.ReadLine();
                // convert string to number
                validResponse = int.TryParse(guessInput, out customerNumber);
            } while (!validResponse);


            do
            {
                Console.WriteLine(@"
                        Current saldo?
                ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = decimal.TryParse(guessInput, out currentSaldo);
            } while (!validResponse);


            if (confirmInput())
            {
                var henkilö = context.customers.FirstOrDefault<customers>
                    (x => x.customer_number.Equals(customerNumber));

                var newAccount = new accounts()
                {
                    account_name = henkilö.customer_last_name,
                    account_type = BankDefs.BankAccount,
                    account_saldo = currentSaldo,
                    credit_limit = creditLimit,
                    customer_number = customerNumber
                };
                context.accounts.Add(newAccount);
                context.SaveChanges();
            }
        }

        private static void GUICreateCreditAccount()
        {
            bool validResponse = false;

            int customerNumber = default;
            decimal creditLimit = default;
            decimal currentSaldo = default;

            GUIShowCustomer();

            do
            {
                Console.WriteLine(@"
                        Who gets new account (customer number)?
                ");
                string guessInput = Console.ReadLine();
                // convert string to number
                validResponse = int.TryParse(guessInput, out customerNumber);
            } while (!validResponse);

            do
            {
                Console.WriteLine(@"
                    Credit Limit?
            ");
                string guessInput = Console.ReadLine();
                // convert string to number
                validResponse = decimal.TryParse(guessInput, out creditLimit);
            } while (!validResponse);

            do
            {
                Console.WriteLine(@"
                        Current saldo?
                ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = decimal.TryParse(guessInput, out currentSaldo);
            } while (!validResponse);


            if (confirmInput())
            {
                var henkilö = context.customers.FirstOrDefault<customers>
                    (x => x.customer_number.Equals(customerNumber));

                var newAccount = new accounts()
                {
                    account_name = henkilö.customer_last_name,
                    account_type = BankDefs.CreditAccount,
                    account_saldo = currentSaldo,
                    credit_limit = creditLimit,
                    customer_number = customerNumber
                };
                context.accounts.Add(newAccount);
                context.SaveChanges();
            }
        }

        private static void GUIMoveAccountToNewCustomer()
        {
            Console.WriteLine(@"
                        Move Account to new Customer
        ");

            GUIShowAccount();
            GUIShowCustomer();

            bool validResponse = false;
            int response = default;
            int response2 = default;

            do
            {
                Console.WriteLine(@"
                        Select Account number to be moved
                    ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response);
            } while (!validResponse);

            do
            {
                Console.WriteLine(@"
                        Select new Customer to receive account
                    ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response2);
            } while (!validResponse);

            if (confirmInput())
            {
                accounts dummy1 = context.accounts.Find(response);
                dummy1.customer_number = response2;
                context.SaveChanges();
                Console.WriteLine($"Account {response} has new customer...");
            }


        }

        private static void GUIShowCustomer()
        {
            Console.WriteLine(@"
                        Customer List:
        ");
            var list =
                from customer in context.customers
                select new
                {
                    Customer = customer.customer_number + 
                    " " + customer.customer_first_name +
                    " " + customer.customer_last_name,
                    Account = customer.customer_number
                };

            foreach (var iter in list)
            {
                Console.Write("  " + iter.Customer);
                Console.Write(" Tilisi ovat: ");

            foreach(accounts iter2 in context.accounts)
            {
                if (iter2.customer_number.Equals(iter.Account))
                {
                    Console.Write(iter2.account_number + " ");
                }
            }
                Console.WriteLine("");
            }

            Console.WriteLine("Press Key to continue");
            Console.ReadLine();
        }


        private static void GUIShowAccount()
        {
            Console.WriteLine(@"
                        Account List:
            ");

            foreach (accounts iter in context.accounts)
            {
                Console.Write(iter.account_number + " : ");
                Console.Write(iter.account_name + " : ");
                Console.WriteLine(iter.account_saldo + " ");
            }

            Console.WriteLine("Press Key to continue");
            Console.ReadLine();
        }

        private static void GUIDeleteCustomer()
        {
            GUIShowCustomer();
            bool validResponse = false;
            int response;
            do
            {
                Console.WriteLine(@"
                        Select Customernumber to be deleted
                    ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response);
            } while (!validResponse);


            if (confirmInput())
            {
                var dummy = context.customers.FirstOrDefault<customers>
                    (x => x.customer_number.Equals(response));

                var dummy2 = context.accounts.FirstOrDefault<accounts>
                    (x => x.customer_number.Equals(response));

                if (dummy2 is null)
                {
                    context.customers.Remove(dummy);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Cannot remove customer with account!");
                }
            }

        }

        private static void GUIDeleteAccount()
        {
            GUIShowAccount();
            bool validResponse = false;
            int response;
            do
            {
                Console.WriteLine(@"
                        Select Account number to be deleted
                    ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response);
            } while (!validResponse);

            if (confirmInput())
            {
            var dummy = context.accounts.FirstOrDefault<accounts>
                (x => x.account_number.Equals(response));

            context.accounts.Remove(dummy);
            context.SaveChanges();
            }
        }

        private static void GUIChangeSaldo()
        {
            GUIShowAccount();

            bool validResponse = false;
            int response = default;
            decimal response2 = default;
            do
            {
                Console.WriteLine(@"
                        Select Account?
        ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = int.TryParse(guessInput, out response);
            } while (!validResponse);

            do
            {
                Console.WriteLine(@"
                        Initial change in saldo (negative for withdrawal)?
        ");
                string guessInput = Console.ReadLine();

                // convert string to number
                validResponse = decimal.TryParse(guessInput, out response2);
            } while (!validResponse);

            if (confirmInput())
            {
                accounts dummy = context.accounts.FirstOrDefault<accounts>
                    (x => x.account_number.Equals(response));

                if (dummy.account_type.Equals(BankDefs.BankAccount))
                {
                    dummy.account_saldo += response2;
                    context.SaveChanges();
                    Console.WriteLine($"Account {response} saldo changed...");
                }
                else
                {
                    decimal? dummy2 = dummy.account_saldo + response2;
                    if (dummy2 >= 0)
                    {
                        dummy.account_saldo += response2;
                        context.SaveChanges();
                        Console.WriteLine($"Account {response} saldo changed...");
                    }
                    else if ( dummy2 >= -dummy.credit_limit )
                    {
                        dummy.account_saldo += response2;
                        context.SaveChanges();
                        Console.WriteLine($"Account uses now credit...");
                    }
                    else
                    {
                        Console.WriteLine($"Account saldo cannot go that low...");
                    }



                }

            }
        }

        private static bool confirmInput()
        {
            bool response = default;

            Console.WriteLine("Confirm Y/N?");
            string confirmInput = Console.ReadLine();
            if (confirmInput.ToUpper() == "Y")
            {
                return true;
            }
            return response;
        }

    }
}
