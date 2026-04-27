using System.Collections.Generic;
using System.ServiceModel;

// Define el "contrato" RMI.
[ServiceContract]
public interface IPersonaController
{
    // En C# 8+ se permiten constantes en las interfaces
    public const int ADD_EXITO = 1;
    public const int ADD_ID_DUPLICADO = 2;
    public const int UPDATE_EXITO = 1;
    public const int UPDATE_NULO = 2;
    public const int UPDATE_ID_INEXISTENTE = 3;
    public const int UPDATE_SIN_EXITO = 4;
    public const int DELETE_EXITO = 1;
    public const int DELETE_ID_INEXISTENTE = 2;
    public const int DELETE_ID_NULO = 3;
    public const int DELETE_SIN_EXITO = 4;

    // En el video utiliza un parámetro IPersona, en WCF es mucho más limpio
    // usar la clase Persona directamente marcada con [DataContract].
    [OperationContract]
    int Add(Persona persona);

    [OperationContract]
    int Update(Persona persona);

    [OperationContract]
    int Delete(Persona persona);

    [OperationContract]
    List<Persona> List();

    [OperationContract]
    Persona FindOne(int idPersona);

    [OperationContract]
    List<Persona> Find(Persona persona);

    [OperationContract]
    int DeleteById(int idPersona);
}
