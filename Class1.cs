using System;

public class Transaction
{
    private double amount;

    private string description;

    private string label;

    private DateTime transaction_time;

    public double getAmount()
    {
        return this.amount;
    }

    public string getDescription()
    {
        return this.description;
    }

    public string getLabel()
    {
        return this.label;
    }

    public DateTime getTransactionTime() 
    {
        return this.transaction_time;
    }

    public void setAmount(double amount) => this.amount = amount;
    public void setDescription(string desc) => this.description = desc;
    public void setLabel(string label) => this.label = label;


    public Transaction(double amount = 0.0, string description = null, string label = null)
    {
        this.amount = amount;
        this.description = description;
        this.label = label;
    }
}
