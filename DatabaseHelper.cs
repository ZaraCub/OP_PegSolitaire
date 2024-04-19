using System;
using System.Data.SQLite;
using System.IO;

public class DatabaseHelper : IDisposable
{
    private SQLiteConnection _connection;

    // Konstruktor koji inicijalizira ili otvara bazu podataka
    public DatabaseHelper()
    {
        const string DbName = "PegSolitaire.db";

        // Provjera postoji li baza podataka
        if (!File.Exists(DbName))
        {
            // Ako ne postoji, kreiraj novu bazu podataka
            SQLiteConnection.CreateFile(DbName);
            _connection = new SQLiteConnection($"Data Source={DbName};Version=3;");
            _connection.Open();

            // Kreiraj tabelu za pohranu rezultata
            using (var command = new SQLiteCommand("CREATE TABLE Scores (Date TEXT, TimeTaken TEXT)", _connection))
            {
                command.ExecuteNonQuery();
            }
        }
        else
        {
            // Ako baza podataka već postoji, samo je otvori
            _connection = new SQLiteConnection($"Data Source={DbName};Version=3;");
            _connection.Open();
        }
    }

    // Implementacija IDisposable sučelja za čišćenje resursa
    public void Dispose()
    {
        _connection?.Dispose();
    }

    // Dodaje novi rezultat u bazu podataka
    public void AddScore(string timeTakenInSeconds)
    {
        // Upit za umetanje novog reda u tabelu s trenutnim datumom i vremenom
        using (var command = new SQLiteCommand($"INSERT INTO Scores (Date, TimeTaken) VALUES (DateTime('now'), '{timeTakenInSeconds}')", _connection))
        {
            command.ExecuteNonQuery();
        }
    }

    // Dohvaća sve rezultate iz baze podataka
    public SQLiteDataReader GetScores()
    {
        using (var command = new SQLiteCommand("SELECT * FROM Scores", _connection))
        {
            return command.ExecuteReader();
        }
    }

    // Briše sve rezultate iz baze podataka
    public void ResetHighScore()
    {
        using (var command = new SQLiteCommand("DELETE FROM Scores", _connection))
        {
            command.ExecuteNonQuery();
        }
    }
}
