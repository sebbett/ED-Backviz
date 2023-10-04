using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public static class db
{
    public static void DBTest()
    {
        string connectionString = "URI=file:data.db";

        SqliteConnection m_conn = new SqliteConnection(connectionString);
        m_conn.Open();

        IDbCommand m_cmd = m_conn.CreateCommand();

        string sql = "SELECT * FROM factions";
        m_cmd.CommandText= sql;

        IDataReader reader = m_cmd.ExecuteReader();
        while (reader.Read())
        {
            string name = reader.GetString(1);
            string id = reader.GetString(2);
            Debug.Log($"{name}  -  {id}");
        }
        reader.Dispose();
        m_cmd.Dispose();
        m_conn.Close();
    }
    
    public static void InsertFactions(string[] factions)
    {
        string connectionString = "URI=file:data.db";

        SqliteConnection m_conn = new SqliteConnection(connectionString);
        m_conn.Open();

        IDbCommand m_cmd = m_conn.CreateCommand();

        var transaction = m_conn.BeginTransaction();
        foreach (string f in factions)
        {
            //$"INSERT INTO factions (faction_name) VALUES ('{f}')";
            m_cmd.Parameters.Clear();
            m_cmd.CommandText = "INSERT INTO factions (faction_name) VALUES (@faction);";

            m_cmd.Parameters.Add(new SqliteParameter("@faction", f));
            m_cmd.ExecuteNonQuery();
        }
        transaction.Commit();
        
        m_cmd.Dispose();
        m_conn.Close();
    }
}
