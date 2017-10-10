using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Budgeteer
{
    class BudgetMonth
    {
        public BudgetMonth()
        {

        }
        public decimal Baby
        {
            get;
            set;
        }
        public decimal Bills
        {
            get;
            set;
        }
        public decimal Food
        {
            get;
            set;
        }
        public decimal Transport
        {
            get;
            set;
        }
        public decimal Fun
        {
            get;
            set;
        }
        public decimal Misc
        {
            get;
            set;
        }
        public decimal Car
        {
            get;
            set;
        }
        public decimal Health
        {
            get;
            set;
        }
        public decimal Exercise
        {
            get;
            set;
        }
        public decimal Liquor
        {
            get;
            set;
        }
        public decimal Vacation
        {
            get;
            set;
        }
        public decimal Work
        {
            get;
            set;
        }
        public decimal Total
        {
            get;
            set;
        }
        public decimal Income
        {
            get;
            set;
        }
        public decimal Expense
        {
            get
            {
                return (Bills+Food+Transport+Fun+Misc+Car+Health+Exercise+Liquor+Vacation+Work+Baby);
            }
        }

        public override string ToString()
        {
            StringBuilder build = new StringBuilder();
            build.AppendLine("Bills: " + this.Bills);
            build.AppendLine("Food: " + this.Food);
            build.AppendLine("Transport: " + this.Transport);
            build.AppendLine("Fun: " + this.Fun);
            build.AppendLine("Misc: " + this.Misc);
            build.AppendLine("Car: " + this.Car);
            build.AppendLine("Health: " + this.Health);
            build.AppendLine("Exercise: " + this.Exercise);
            build.AppendLine("Liquor: " + this.Liquor);
            build.AppendLine("Vacation: " + this.Vacation);
            build.AppendLine("Work Expenses: " + this.Work);
            build.AppendLine("Baby: " + this.Baby);
            build.AppendLine("Total Expenses: " + this.Total);
            build.AppendLine("Total Category Expenses: " + this.Expense);
            build.AppendLine("Income: " + this.Income);
            build.AppendLine("");
            build.AppendLine("Net: " + (Income - Expense));

            return build.ToString();
        }
    }
}
