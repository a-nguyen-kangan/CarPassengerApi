using Microsoft.AspNetCore.Components.Authorization;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Person p1 = new Person();
Person p2 = new Person(1, "John", 20);

app.MapGet("/", () => p2);
app.MapGet("/people", () => getPeople());
app.MapGet("/people/{id}", (int id) => getPersonById(id));
app.MapGet("/carspassengers", () => getCarsWithPassengers());
app.MapGet("cars/", () => GetAllCars());

//*** Stuff to practice ***
// 1. Create an endpoint that returns all cars (whether they have passengers or not)
// 2. Search for a car by rego
// 3. Search for a person by name (partial name search)

// Post endpoints
app.MapPost("/cars", (Car newCar) => InsertNewCar(newCar));

//*** Stuff to practice ***
// 1. Create an endpoint that allows you to add a new person
// 2. Create an endpoint that allows you to add a person to a car
// 3. Create an endpoint that allows you to add a driver to a car
// 4. Create an endpoint that allows you to remove a person from a car
// 5. Create an endpoint that allows you to remove a driver from a car
// 6. Create endpoints to remove cars and people by rego and id respectively

// Inserts new car into database, if successful return the car
string InsertNewCar(Car newCar)
{
    // connect to the database
        using (var conn = getDbConnection()) {
            try {
                conn.Open();
                // query
                // map values from the newCar object to the query
                string query = $"INSERT INTO \"Car\" (rego, colour, make, model) VALUES (@regoVal, @colourVal, @makeVal, @modelVal)";
                
                using(var cmd = new NpgsqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("regoVal", newCar.Rego);
                    cmd.Parameters.AddWithValue("colourVal", newCar.Colour);
                    cmd.Parameters.AddWithValue("makeVal", newCar.Make);
                    cmd.Parameters.AddWithValue("modelVal", newCar.Model);
                    cmd.ExecuteNonQuery();
                }


            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }


    return newCar.Rego;
}

app.MapPost("/findCarByRego", (Car findCar) => FindCarByRego(findCar));


app.Run();

Car FindCarByRego(Car findCar) {
    return GetAllCars().Find(car => car.Rego == findCar.Rego);
}

Person getPersonById(int id) {
    return getPeople().Find(person => person.Id == id);
}

List<Person> getPeople() {
    List<Person> people = new List<Person>();
    using (var conn = getDbConnection()) {
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

List<Car> GetAllCars() {
    List<Car> cars = new List<Car>();
    using (var conn = getDbConnection()) {
        try {
            conn.Open();
            string query = "Select * From \"Car\"";
            using (var cmd = new NpgsqlCommand(query, conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        cars.Add(new Car(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    return cars;
}

List<Car> getCarsWithPassengers() {
    List<Car> cars = new List<Car>();

     using (var conn = getDbConnection()) {
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

NpgsqlConnection getDbConnection() {
    return new NpgsqlConnection("User Id=postgres;Password=lctasd!#%&(;Server=db.uioaemhkajajdgxqskwv.supabase.co;Port=5432;Database=postgres");
}