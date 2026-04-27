using System;
using System.Collections.Generic;

// Implementa la interfaz anterior. Aquí es donde se escribe la lógica real
// que une a la persona con la base de datos a través del DBManager.
public class PersonaController : IPersonaController
{
    // Utiliza una constante final para no escribir el texto plano del nombre de la tabla repetidas veces.
    public const string TABLA_PERSONAS = "Personas";

    // Atributo privado muy importante
    private DBManager dbManager;

    public PersonaController()
    {
        dbManager = new DBManager();
    }

    public int Add(Persona persona)
    {
        try
        {
            // Validar si la persona ya existe
            var where = new Dictionary<string, object> { { "IdPersona", persona.IdPersona } };
            var registroExistente = dbManager.BuscarUno(TABLA_PERSONAS, where);

            if (registroExistente != null)
            {
                return IPersonaController.ADD_ID_DUPLICADO; // Ya existe el ID
            }

            // Empaquetar datos para el DBManager
            var datos = new Dictionary<string, object>
            {
                { "IdPersona", persona.IdPersona },
                { "Nombre", persona.Nombre },
                { "Email", persona.Email },
                { "Telefono", persona.Telefono }
            };

            dbManager.Insertar(TABLA_PERSONAS, datos);
            return IPersonaController.ADD_EXITO;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al insertar: " + ex.Message);
            return 0; // Código genérico de fallo
        }
    }

    public int Update(Persona persona)
    {
        try
        {
            // 1. Validar antes de actuar (Que no sea nulo el id)
            if (persona == null || persona.IdPersona == 0)
            {
                return IPersonaController.UPDATE_NULO;
            }

            // 1. Validar antes de actuar (Uso de findOne)
            var registroExistente = FindOne(persona.IdPersona);
            if (registroExistente == null)
            {
                return IPersonaController.UPDATE_ID_INEXISTENTE;
            }

            // 2. Refactorización para no repetir código (El método toMap)
            var datos = persona.ToMap();
            
            var where = new Dictionary<string, object>
            {
                { "IdPersona", persona.IdPersona }
            };

            // Ejecuta el DBManager, que devuelve el número de filas afectadas
            int respuesta = dbManager.Actualizar(TABLA_PERSONAS, datos, where);

            // 3. Operador Ternario vs. Condicionales (if/else)
            return (respuesta > 0) ? IPersonaController.UPDATE_EXITO : IPersonaController.UPDATE_SIN_EXITO;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al actualizar: " + ex.Message);
            return IPersonaController.UPDATE_SIN_EXITO; // 4. Nuevos estados de error
        }
    }

    public int Delete(Persona persona)
    {
        try
        {
            if (persona == null || persona.IdPersona == 0)
            {
                return IPersonaController.DELETE_ID_NULO;
            }

            var registroExistente = FindOne(persona.IdPersona);
            if (registroExistente == null)
            {
                return IPersonaController.DELETE_ID_INEXISTENTE;
            }

            var where = new Dictionary<string, object>
            {
                { "IdPersona", persona.IdPersona }
            };

            int respuesta = dbManager.Eliminar(TABLA_PERSONAS, where);
            return (respuesta > 0) ? IPersonaController.DELETE_EXITO : IPersonaController.DELETE_SIN_EXITO;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al eliminar: " + ex.Message);
            return IPersonaController.DELETE_SIN_EXITO;
        }
    }

    public List<Persona> List()
    {
        // Se define el ArrayList (List en C#) donde se irán guardando las personas convertidas.
        var listaPersonas = new List<Persona>();
        
        // Se invoca listar(tabla) en DBManager que ejecuta el "SELECT * FROM Personas"
        var registros = dbManager.Listar(TABLA_PERSONAS);

        // Se iteran los mapas (filas) y se pasan por el "traductor" FromMap uno por uno,
        // usando el método add() de las colecciones para ir agregando personas a la lista final.
        foreach (var fila in registros)
        {
            listaPersonas.Add(Persona.FromMap(fila));
        }

        return listaPersonas;
    }

    public Persona FindOne(int idPersona)
    {
        var where = new Dictionary<string, object> { { "IdPersona", idPersona } };
        var fila = dbManager.BuscarUno(TABLA_PERSONAS, where);

        if (fila != null)
        {
            // Pasamos el diccionario devuelto por el DBManager a nuestro traductor FromMap
            return Persona.FromMap(fila);
        }

        return null;
    }

    public List<Persona> Find(Persona persona)
    {
        var listaPersonas = new List<Persona>();
        
        if (persona == null) 
        {
            return listaPersonas;
        }

        // ToMap() ahora solo trae los campos con datos (ej. solo el email)
        var where = persona.ToMap(); 
        
        // Ejecutamos listar pasándole el mapa de condiciones
        var registros = dbManager.Listar(TABLA_PERSONAS, where);

        // Iteramos los resultados, los convertimos a objetos y los agregamos a la lista
        foreach (var fila in registros)
        {
            listaPersonas.Add(Persona.FromMap(fila));
        }

        return listaPersonas;
    }

    public int Delete(int idPersona)
    {
        // Reto: Reciclar la lógica de Delete(Persona persona)
        var persona = new Persona { IdPersona = idPersona };
        return Delete(persona);
    }
}
