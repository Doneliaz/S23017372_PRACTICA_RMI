using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ClienteRMI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Cliente RMI de Prueba ---");
            Console.WriteLine("Intentando conectar al Servidor...");
            
            // Configuramos la conexión hacia el Servidor RMI (TCP Local sin seguridad)
            var binding = new NetTcpBinding(SecurityMode.None);
            var endpoint = new EndpointAddress("net.tcp://localhost:1099/PersonaController");
            
            // ChannelFactory es el equivalente en WCF a Naming.lookup("rmi://...") de Java
            var factory = new ChannelFactory<IPersonaController>(binding, endpoint);
            IPersonaController servidorPersonas = factory.CreateChannel();

            try
            {
                bool salir = false;
                while (!salir)
                {
                    Console.WriteLine("\n===========================");
                    Console.WriteLine("        MENÚ CRUD");
                    Console.WriteLine("===========================");
                    Console.WriteLine("1. Crear Persona");
                    Console.WriteLine("2. Listar Personas");
                    Console.WriteLine("3. Actualizar Persona");
                    Console.WriteLine("4. Eliminar Persona");
                    Console.WriteLine("5. Buscar Persona");
                    Console.WriteLine("6. Salir");
                    Console.Write("Seleccione una opción: ");
                    string opcion = Console.ReadLine();

                    switch (opcion)
                    {
                        case "1":
                            CrearPersona(servidorPersonas);
                            break;
                        case "2":
                            ListarPersonas(servidorPersonas);
                            break;
                        case "3":
                            ActualizarPersona(servidorPersonas);
                            break;
                        case "4":
                            EliminarPersona(servidorPersonas);
                            break;
                        case "5":
                            BuscarPersona(servidorPersonas);
                            break;
                        case "6":
                            salir = true;
                            Console.WriteLine("Saliendo del programa...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Intente nuevamente.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError al conectar o recuperar datos. ¿El ServidorRMI está encendido?");
                Console.WriteLine("Detalle: " + ex.Message);
            }
            finally
            {
                // Cerramos los canales de red
                if (servidorPersonas is IClientChannel channel)
                {
                    channel.Close();
                }
                factory.Close();
            }
        }

        static void CrearPersona(IPersonaController servidor)
        {
            Console.WriteLine("\n--- Crear Persona ---");
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine();

            Persona p = new Persona { Nombre = nombre, Email = email, Telefono = telefono };
            int result = servidor.Add(p);

            if (result == IPersonaController.ADD_EXITO)
                Console.WriteLine("¡Persona creada con éxito!");
            else if (result == IPersonaController.ADD_ID_DUPLICADO)
                Console.WriteLine("Error: El ID ya existe (Duplicado).");
            else
                Console.WriteLine("Error al crear la persona. Código de error: " + result);
        }

        static void ListarPersonas(IPersonaController servidor)
        {
            Console.WriteLine("\n--- Lista de Personas ---");
            List<Persona> lista = servidor.List();
            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No hay personas registradas en la base de datos.");
            }
            else
            {
                Console.WriteLine($"Se encontraron {lista.Count} registros:");
                foreach (var p in lista)
                {
                    Console.WriteLine($" - ID: {p.IdPersona} | Nombre: {p.Nombre} | Email: {p.Email} | Teléfono: {p.Telefono}");
                }
            }
        }

        static void ActualizarPersona(IPersonaController servidor)
        {
            Console.WriteLine("\n--- Actualizar Persona ---");
            Console.Write("Ingrese el ID de la persona a actualizar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Nuevo Nombre: ");
                string nombre = Console.ReadLine();
                Console.Write("Nuevo Email: ");
                string email = Console.ReadLine();
                Console.Write("Nuevo Teléfono: ");
                string telefono = Console.ReadLine();

                Persona p = new Persona { IdPersona = id, Nombre = nombre, Email = email, Telefono = telefono };
                int result = servidor.Update(p);

                if (result == IPersonaController.UPDATE_EXITO)
                    Console.WriteLine("¡Persona actualizada con éxito!");
                else if (result == IPersonaController.UPDATE_ID_INEXISTENTE)
                    Console.WriteLine("Error: El ID ingresado no existe.");
                else
                    Console.WriteLine("Error al actualizar la persona. Código de error: " + result);
            }
            else
            {
                Console.WriteLine("ID inválido. Debe ser un número entero.");
            }
        }

        static void EliminarPersona(IPersonaController servidor)
        {
            Console.WriteLine("\n--- Eliminar Persona ---");
            Console.Write("Ingrese el ID de la persona a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                int result = servidor.DeleteById(id);

                if (result == IPersonaController.DELETE_EXITO)
                    Console.WriteLine("¡Persona eliminada con éxito!");
                else if (result == IPersonaController.DELETE_ID_INEXISTENTE)
                    Console.WriteLine("Error: El ID ingresado no existe.");
                else
                    Console.WriteLine("Error al eliminar la persona. Código de error: " + result);
            }
            else
            {
                Console.WriteLine("ID inválido. Debe ser un número entero.");
            }
        }

        static void BuscarPersona(IPersonaController servidor)
        {
            Console.WriteLine("\n--- Buscar Persona ---");
            Console.WriteLine("1. Buscar por ID exacto");
            Console.WriteLine("2. Buscar por coincidencias (Nombre, Email, Teléfono)");
            Console.Write("Seleccione una opción de búsqueda: ");
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("Ingrese el ID a buscar: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Persona p = servidor.FindOne(id);
                    if (p != null)
                    {
                        Console.WriteLine("\nRegistro encontrado:");
                        Console.WriteLine($" - ID: {p.IdPersona} | Nombre: {p.Nombre} | Email: {p.Email} | Teléfono: {p.Telefono}");
                    }
                    else
                    {
                        Console.WriteLine("No se encontró ninguna persona con ese ID.");
                    }
                }
                else
                {
                    Console.WriteLine("ID inválido. Debe ser un número entero.");
                }
            }
            else if (op == "2")
            {
                Persona buscarPersona = new Persona();
                Console.WriteLine("Ingrese los datos a buscar (deje en blanco si no desea filtrar por ese campo):");
                
                Console.Write("Nombre: ");
                string nombre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nombre)) buscarPersona.Nombre = nombre;

                Console.Write("Email: ");
                string email = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(email)) buscarPersona.Email = email;

                Console.Write("Teléfono: ");
                string telefono = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(telefono)) buscarPersona.Telefono = telefono;

                List<Persona> encontrados = servidor.Find(buscarPersona);
                if (encontrados != null && encontrados.Count > 0)
                {
                    Console.WriteLine($"\nSe encontraron {encontrados.Count} coincidencias:");
                    foreach (var p in encontrados)
                    {
                        Console.WriteLine($" - ID: {p.IdPersona} | Nombre: {p.Nombre} | Email: {p.Email} | Teléfono: {p.Telefono}");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron coincidencias con los criterios dados.");
                }
            }
            else
            {
                Console.WriteLine("Opción de búsqueda no válida.");
            }
        }
    }
}
