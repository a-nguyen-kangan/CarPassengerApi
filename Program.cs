using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Person p1 = new Person();
Person p2 = new Person(1, "John", 20);

app.MapGet("/", () => p2);
app.MapGet("/people", () => getPeople());
app.MapGet("/carspassengers", () => getCarsWithPassengers());

app.Run();

List<Person> getPeople() {
    List<Person> people = new List<Person>();
    using (var conn = new NpgsqlConnection("User Id=postgres;Password=lctasd!#%&(;Server=db.uioaemhkajajdgxqskwv.supabase.co;Port=5432;Database=postgres")) {
        try {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT * FROM \"Person\"", conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        people.Add(new Person(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
    return people;

}

List<Car> getCarsWithPassengers() {
    List<Car> cars = new List<Car>();

     using (var conn = new NpgsqlConnection("User Id=postgres;Password=lctasd!#%&(;Server=db.uioaemhkajajdgxqskwv.supabase.co;Port=5432;Database=postgres")) {
        try {
            conn.Open();
            string query = "Select * From \"Car\" Join \"Person\" on \"Car\".rego = \"Person\".rego";
             using (var cmd = new NpgsqlCommand(query, conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        Car tempCar = new Car(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        
                        Person tempPerson = new Person(reader.GetInt32(4), reader.GetString(5), reader.GetInt32(6));
                        
                        if(!carInList(cars, tempCar)) {
                            cars.Add(tempCar);
                        }

                        // Add person to the correct car
                        int carIndex = getCarIndexFromRego(cars, tempCar.Rego);

                        if(carIndex != -1) {
                            cars[carIndex].Passengers.Add(tempPerson);
                        }

                        // Add driver to the correct car
                        if (reader.GetBoolean(8)) {
                          cars[carIndex].Driver = tempPerson;
                        } 
                            
                    }
                }
             }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
     }
        return cars;
}

bool carInList(List<Car> cars, Car newCar) {
    foreach (Car c in cars) {
        if (c.Rego == newCar.Rego) {
            return true;
        }
    }
    return false;
}

int getCarIndexFromRego(List<Car> car, string rego) {
    for (int i = 0; i < car.Count; i++) {
        if (car[i].Rego == rego) {
            return i;
        }
    }
    return -1;
}