using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BankAccountApp
{
    public class InsufficientFundsException : Exception
    {
        public double AttemptedAmount { get; }
        public double CurrentBalance { get; }

        public InsufficientFundsException(double attemptedAmount, double currentBalance)
            : base("Insufficient balance for debit.")
        {
            AttemptedAmount = attemptedAmount;
            CurrentBalance = currentBalance;
        }
    }

    public class InvalidAccountTypeException : Exception
    {
        public string AccountType { get; }

        public InvalidAccountTypeException(string accountType)
            : base("Invalid account type.")
        {
            AccountType = accountType;
        }
    }

    public class InvalidAccountNumberException : Exception
    {
        public string AccountNumber { get; }

        public InvalidAccountNumberException(string accountNumber)
            : base("Invalid account number.")
        {
            AccountNumber = accountNumber;
        }
    }

    public class InvalidAccountHolderNameException : Exception
    {
        public string AccountHolderName { get; }

        public InvalidAccountHolderNameException(string accountHolderName)
            : base("Invalid account holder name.")
        {
            AccountHolderName = accountHolderName;
        }
    }

    public class InvalidDateOpenedException : Exception
    {
        public DateTime DateOpened { get; }

        public InvalidDateOpenedException(DateTime dateOpened)
            : base("Invalid date opened.")
        {
            DateOpened = dateOpened;
        }
    }

    public class InvalidInitialBalanceException : Exception
    {
        public double InitialBalance { get; }

        public InvalidInitialBalanceException(double initialBalance)
            : base("Invalid initial balance.")
        {
            InitialBalance = initialBalance;
        }
    }

    public class InvalidInterestRateException : Exception
    {
        public double InterestRate { get; }

        public InvalidInterestRateException(double interestRate)
            : base("Invalid interest rate.")
        {
            InterestRate = interestRate;
        }
    }

    public class InvalidTransferAmountException : Exception
    {
        public double TransferAmount { get; }

        public InvalidTransferAmountException(double transferAmount)
            : base("Invalid transfer amount.")
        {
            TransferAmount = transferAmount;
        }
    }

    public class InvalidCreditAmountException : Exception
    {
        public double CreditAmount { get; }

        public InvalidCreditAmountException(double creditAmount)
            : base("Invalid credit amount.")
        {
            CreditAmount = creditAmount;
        }
    }

    public class InvalidDebitAmountException : Exception
    {
        public double DebitAmount { get; }

        public InvalidDebitAmountException(double debitAmount)
            : base("Invalid debit amount.")
        {
            DebitAmount = debitAmount;
        }
    }

    public class BankAccount
    {
        public enum AccountTypes
        {
            Savings,
            Checking,
            MoneyMarket,
            CertificateOfDeposit,
            Retirement
        }
        public string AccountNumber { get; }
        
        
        public string GetAccountNumber()
        {
                return AccountNumber;
        }
        
        

        public double Balance { get; private set; }
        public string AccountHolderName { get; }

        public string GetAccountHolderName()
        {
                return AccountHolderName;
        }
        public AccountTypes AccountType { get; }
        public DateTime DateOpened { get; }
        private const double MaxTransferAmountForDifferentOwners = 500;
        private string EncryptedPin { get; }

        public BankAccount(string accountNumber, double initialBalance, string accountHolderName, string accountType, DateTime dateOpened, string pin)
        {
            if (accountNumber.Length != 10)
            {
                throw new InvalidAccountNumberException(accountNumber);
            }

            if (initialBalance < 0)
            {
                throw new InvalidInitialBalanceException(initialBalance);
            }

            if (accountHolderName.Length < 2)
            {
                throw new InvalidAccountHolderNameException(accountHolderName);
            }

            if (dateOpened > DateTime.Now)
            {
                throw new InvalidDateOpenedException(dateOpened);
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
            {
                throw new ArgumentException("PIN must be at least 4 characters long.");
            }

            AccountNumber = accountNumber;
            Balance = initialBalance;
            AccountHolderName = accountHolderName;
            AccountType = (AccountTypes)Enum.Parse(typeof(AccountTypes), accountType);
            DateOpened = dateOpened;
            EncryptedPin = EncryptPin(pin);
        }

        private string EncryptPin(string pin)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(pin);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPin(string pin)
        {
            var encryptedPin = EncryptPin(pin);
            return EncryptedPin == encryptedPin;
        }

        private void Authenticate(string pin)
        {
            if (!VerifyPin(pin))
            {
                throw new UnauthorizedAccessException("Invalid PIN.");
            }
        }

        public void Credit(double amount, string pin)
        {
            Authenticate(pin);

            if (amount < 0)
            {
                throw new InvalidCreditAmountException(amount);
            }

            Balance += amount;
        }

        public void Debit(double amount, string pin)
        {
            Authenticate(pin);

            if (amount < 0)
            {
                throw new InvalidDebitAmountException(amount);
            }

            if (Balance >= amount)
            {
                Balance -= amount;
            }
            else
            {
                LogException(new InsufficientFundsException(amount, Balance));
                throw new InsufficientFundsException(amount, Balance);
            }
        }

        public double GetBalance()
        {
            return Balance;
        }

        public void Transfer(BankAccount toAccount, double amount, string pin)
        {
            Authenticate(pin);
            ValidateTransferAmount(amount);
            ValidateTransferLimitForDifferentOwners(toAccount, amount);

            if (Balance >= amount)
            {
                Debit(amount, pin);
                toAccount.Credit(amount, pin);
            }
            else
            {
                LogException(new InsufficientFundsException(amount, Balance));
                throw new InsufficientFundsException(amount, Balance);
            }
        }

        private void ValidateTransferAmount(double amount)
        {
            if (amount < 0)
            {
                LogException(new InvalidTransferAmountException(amount));
                throw new InvalidTransferAmountException(amount);
            }
        }

        private void ValidateTransferLimitForDifferentOwners(BankAccount toAccount, double amount)
        {
            if (AccountHolderName != toAccount.AccountHolderName && amount > MaxTransferAmountForDifferentOwners)
            {
                LogException(new Exception("Transfer amount exceeds maximum limit for different account owners."));
                throw new Exception("Transfer amount exceeds maximum limit for different account owners.");
            }
        }

        private void LogException(Exception ex)
        {
            string logFilePath = "exception_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {ex.GetType().Name} - {ex.Message}");
                writer.WriteLine(ex.StackTrace);
            }
        }

        public void PrintStatement()
        {
            Console.WriteLine($"Account Number: {AccountNumber}, Balance: {Balance}");
            // Add code here to print recent transactions
        }

        public double CalculateInterest(double interestRate)
        {
            if (interestRate < 0)
            {
                throw new InvalidInterestRateException(interestRate);
            }

            return Balance * interestRate;
        }
    }
}