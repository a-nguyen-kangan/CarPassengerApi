public class Car {
    public string Rego { get; set; }
    public string Colour { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public List<Person> Passengers { get; set; }
    public Person Driver { get; set; }

    public Car() {
        // This is required for Entity Framework
    }

    public Car(string rego, string colour, string make, string model) {
        Rego = rego;
        Colour = colour;
        Make = make;
        Model = model;
        Passengers = new List<Person>();
        Driver = null;
    }
}