using System.Runtime.Serialization;

// En WCF (el RMI de C#), para que un modelo de datos pueda viajar por la red,
// se decora con [DataContract] y sus atributos con [DataMember].
// En el video de Java hereda de UnicastRemoteObject (paso por referencia), 
// pero en C# (y en las arquitecturas modernas) los modelos viajan por valor (serializados).
[DataContract]
public class Persona
{
    [DataMember]
    public int IdPersona { get; set; }

    [DataMember]
    public string Nombre { get; set; }

    [DataMember]
    public string Email { get; set; }

    [DataMember]
    public string Telefono { get; set; }

    // Constructores
    public Persona() { }

    public Persona(int idPersona, string nombre, string email, string telefono)
    {
        IdPersona = idPersona;
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    // El método estático equivalente a fromMap(Map map) mencionado en el video.
    // Toma un HashMap (Dictionary) crudo de la base de datos y lo convierte en el objeto Persona.
    public static Persona FromMap(System.Collections.Generic.Dictionary<string, object> map)
    {
        var persona = new Persona();

        // Se usa ContainsKey para asegurar que la columna existe en el resultado, 
        // seguido del "Casteo/Promoción" de tipos sugerido (Convert.ToInt32 y ToString())
        if (map.ContainsKey("IdPersona") && map["IdPersona"] != null)
        {
            persona.IdPersona = System.Convert.ToInt32(map["IdPersona"]);
        }
        
        if (map.ContainsKey("Nombre") && map["Nombre"] != null)
        {
            persona.Nombre = map["Nombre"].ToString();
        }

        if (map.ContainsKey("Email") && map["Email"] != null)
        {
            persona.Email = map["Email"].ToString();
        }

        if (map.ContainsKey("Telefono") && map["Telefono"] != null)
        {
            persona.Telefono = map["Telefono"].ToString();
        }

        return persona;
    }

    public System.Collections.Generic.Dictionary<string, object> ToMap()
    {
        var map = new System.Collections.Generic.Dictionary<string, object>();
        
        if (!string.IsNullOrEmpty(Nombre)) map.Add("Nombre", Nombre);
        if (!string.IsNullOrEmpty(Email)) map.Add("Email", Email);
        if (!string.IsNullOrEmpty(Telefono)) map.Add("Telefono", Telefono);

        return map;
    }
}
