public class Person {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int ProofNum { get; set; }

    public Person() {
        // This is required for Entity Framework
    }

    public Person(int id, string name, int age) {
        Id = id;
        Name = name;
        Age = age;
        ProofNum = 0;
    }

}