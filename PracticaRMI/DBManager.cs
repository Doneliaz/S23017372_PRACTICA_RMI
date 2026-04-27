using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

public class DBManager
{
    // Equivalente a private Connection conexion;
    private SqliteConnection conexion;

    public DBManager()
    {
        // En Java se usa DriverManager.getConnection(url)
        // Usamos BaseDirectory para encontrar la base de datos de manera consistente
        string dbPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "db.sqlite");
        string url = $"Data Source={dbPath}";
        conexion = new SqliteConnection(url);
        conexion.Open();
    }

    // --- MÉTODOS AUXILIARES ---

    // Este método auxiliar construye la cláusula WHERE dinámicamente.
    private string ConstruirWhere(Dictionary<string, object> where, string sufijoParametro = "")
    {
        if (where == null || where.Count == 0) return "";
        var condiciones = where.Keys.Select(k => $"{k} = @{k}{sufijoParametro}");
        return " WHERE " + string.Join(" AND ", condiciones);
    }

    // Este método auxiliar agrega los parámetros para evitar inyección SQL (Mejor práctica equivalente a PreparedStatement)
    private void AgregarParametros(SqliteCommand comando, Dictionary<string, object> parametros, string sufijoParametro = "")
    {
        if (parametros != null)
        {
            foreach (var kvp in parametros)
            {
                comando.Parameters.AddWithValue($"@{kvp.Key}{sufijoParametro}", kvp.Value ?? DBNull.Value);
            }
        }
    }


    // --- OPERACIONES CRUD ---

    // 1. Insertar
    public void Insertar(string tabla, Dictionary<string, object> datos)
    {
        if (datos == null || datos.Count == 0) return;

        var columnas = string.Join(", ", datos.Keys);
        var valores = string.Join(", ", datos.Keys.Select(k => "@" + k));
        
        // Equivalente a Statement o PreparedStatement
        string sql = $"INSERT INTO {tabla} ({columnas}) VALUES ({valores})";

        using (var comando = new SqliteCommand(sql, conexion))
        {
            AgregarParametros(comando, datos);
            comando.ExecuteNonQuery(); // Ejecuta la instrucción sin esperar un ResultSet
        }
    }

    // 2. Actualizar (Versión con condición WHERE)
    public int Actualizar(string tabla, Dictionary<string, object> datos, Dictionary<string, object> where)
    {
        if (datos == null || datos.Count == 0) return 0;

        var setClause = string.Join(", ", datos.Keys.Select(k => $"{k} = @{k}"));
        string whereClause = ConstruirWhere(where, "_w");

        string sql = $"UPDATE {tabla} SET {setClause}{whereClause}";

        using (var comando = new SqliteCommand(sql, conexion))
        {
            AgregarParametros(comando, datos);
            // Diferenciamos los nombres de los parámetros del WHERE en caso de que coincidan con los de DATOS
            AgregarParametros(comando, where, "_w"); 
            return comando.ExecuteNonQuery();
        }
    }

    // 2. Actualizar (Versión sin condición WHERE, afecta a todos)
    public int Actualizar(string tabla, Dictionary<string, object> datos)
    {
        return Actualizar(tabla, datos, null);
    }

    // 3. Eliminar (Versión con condición WHERE)
    public int Eliminar(string tabla, Dictionary<string, object> where)
    {
        string whereClause = ConstruirWhere(where);
        string sql = $"DELETE FROM {tabla}{whereClause}";

        using (var comando = new SqliteCommand(sql, conexion))
        {
            AgregarParametros(comando, where);
            return comando.ExecuteNonQuery();
        }
    }

    // 3. Eliminar (Versión sin condición WHERE, afecta a todos)
    public int Eliminar(string tabla)
    {
        return Eliminar(tabla, null);
    }

    // 4. Listar (Equivalente a retornar List<Map<String, Object>>)
    public List<Dictionary<string, object>> Listar(string tabla, Dictionary<string, object> where = null)
    {
        string whereClause = ConstruirWhere(where);
        string sql = $"SELECT * FROM {tabla}{whereClause}";
        
        var resultados = new List<Dictionary<string, object>>();

        using (var comando = new SqliteCommand(sql, conexion))
        {
            AgregarParametros(comando, where);

            // Equivalente a java.sql.ResultSet
            using (var lector = comando.ExecuteReader())
            {
                while (lector.Read()) // Equivalente a resultSet.next()
                {
                    var fila = new Dictionary<string, object>();
                    // Equivalente a resultSet.getMetaData() para obtener nombres de columnas
                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        fila[lector.GetName(i)] = lector.GetValue(i);
                    }
                    resultados.Add(fila);
                }
            }
        }
        return resultados;
    }

    // 5. BuscarUno (Equivalente a obtener el primer registro)
    public Dictionary<string, object> BuscarUno(string tabla, Dictionary<string, object> where = null)
    {
        string whereClause = ConstruirWhere(where);
        string sql = $"SELECT * FROM {tabla}{whereClause} LIMIT 1"; // SQLite soporta LIMIT 1

        using (var comando = new SqliteCommand(sql, conexion))
        {
            AgregarParametros(comando, where);

            using (var lector = comando.ExecuteReader())
            {
                if (lector.Read()) // Lee el primer resultado
                {
                    var fila = new Dictionary<string, object>();
                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        fila[lector.GetName(i)] = lector.GetValue(i);
                    }
                    return fila;
                }
            }
        }
        
        return null; // Si no encontró registros, retorna nulo
    }
}
