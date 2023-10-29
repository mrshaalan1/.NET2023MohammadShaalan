namespace ShoppingCart.Models;
using System.Runtime.Serialization;

[DataContract]
public class DataPoint
{
    public DataPoint(int x, decimal y)
    {
        this.x = x;
        this.Y = y;
    }
    
    public DataPoint(string label, double sold)
    {
        this.Label = label;
        this.Sold = sold;
    }
 
    //Explicitly setting the name to be used while serializing to JSON.
    [DataMember(Name = "x")]
    public Nullable<int> x = null;
 
    //Explicitly setting the name to be used while serializing to JSON.
    [DataMember(Name = "y")]
    public Nullable<decimal> Y = null;
    
    //Explicitly setting the name to be used while serializing to JSON.
    [DataMember(Name = "label")]
    public string Label = "";
 
    //Explicitly setting the name to be used while serializing to JSON.
    [DataMember(Name = "sold")]
    public Nullable<double> Sold = null;
}