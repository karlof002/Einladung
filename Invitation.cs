using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Invitation
{
	public class Invitation
	{
		/// <summary>
		/// Einzuladende Person
		/// </summary>
	    struct Friend
		{
		    public string LastName;
			public string FirstName;
			public string Street;
			public int ZipCode;
			public string City;
            public int NumberOfInvitations;
		}

		private static void Main(string[] args)
		{
            string fileName;
            Friend[] newList;

            Friend[] invitations = new Friend[100]; // Zu erzeugende Liste
            Console.WindowWidth = 120;
            Console.WriteLine("Einladungsliste aus mehreren Detaillisten zusammensetzen");
			Console.WriteLine();
			Console.Write("Bitte Listennamen eingeben (leere Eingabe zum Beenden) ");
			fileName = Console.ReadLine(); 
			while (fileName != "")  // Bis Benutzer eine leere Eingabe macht, Dateien einlesen und mischen
			{
				newList = ReadList(fileName);  // Neue Liste aus Dateiinhalt erzeugen
			    invitations = MergeList(invitations, newList);  // Gesamtliste mit neuer Liste mischen
				Console.Write("Bitte Listennamen eingeben (leere Eingabe zum Beenden) ");
				fileName = Console.ReadLine(); 
			}
            OrderPers(invitations);
            WriteList(invitations); // Gesamtliste auf Bildschirm und auf Datei ausgeben
			Console.ReadLine();
		}

		/// <summary>
		/// Ausgabe der Einladungsliste auf den Bildschirm und in die 
		/// Datei Einladung.txt
		/// </summary>
		/// <param name="invitationList">Auszugebende Liste</param>
		private static void WriteList(Friend[] invitationList)
		{
            string text="";
			if (invitationList != null)
			{
                
				Console.WriteLine();
                Console.WriteLine("{0,-28} {1,-30} {2,-4} {3, -25} {4, -11}", "Name", "Straße", "PLZ", "Ort", "Anz. Einl.");
				// Listenende ist erreicht, wenn kein Nachname mehr gespeichert ist
                int idx = 0;
                
                while ((idx < (int)invitationList.Length) && (invitationList[idx].LastName!=null))
				{
                    Console.WriteLine("{0,-28} {1,-30} {2,-4} {3, -25} {4, -11}", invitationList[idx].LastName + " " + invitationList[idx].FirstName,
                        invitationList[idx].Street, invitationList[idx].ZipCode, invitationList[idx].City, invitationList[idx].NumberOfInvitations);
                    text += string.Format("{0};{1};{2};{3};{4};{5}", invitationList[idx].LastName, invitationList[idx].FirstName,
                        invitationList[idx].Street, invitationList[idx].ZipCode, invitationList[idx].City, invitationList[idx].NumberOfInvitations)+Environment.NewLine;
                    idx++;
				}
				Console.WriteLine();
                //Datei erstellen
                File.WriteAllText("Einladung.csv", text, Encoding.Default);
			}
		}

        /// <summary>
        /// Sortiert die Personen nach der Familienname (Insertion-Sort)
        /// </summary>
        /// <param name="rooms"></param>
        static void OrderPers(Friend[] friends)
        {
            // a.CompareTo(b) ==>  ==0, wenn a == b
            //                     >0, wenn a > b
            //                    <0, wenn a < b  

            Friend temp;
            int actPos;
            int i=1;
            while (friends[i].LastName!=null)
            {
                temp = friends[i];
                actPos = i - 1;
                while (actPos >= 0 && friends[actPos].LastName.CompareTo(temp.LastName)>0)
                {
                    friends[actPos + 1] = friends[actPos];
                    actPos--;
                }
                friends[actPos + 1] = temp;
                i++;
            } 
        }

		/// <summary>
		/// Liste aus Textdatei erzeugen
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		static Friend[] ReadList(string fileName)
		{
			Friend[] newList=null;
            fileName = fileName + ".csv";
            if (File.Exists(fileName))  // Prüfen ob Datei vorhanden
            {
                string[] lines = File.ReadAllLines(fileName, Encoding.Default);
                newList = new Friend[lines.Length];  // Zielliste erzeugen
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    // Zeile aufsplitten und in Struktur übertragen
                    string[] columns = line.Split(';');
                    newList[i].LastName = columns[0];
                    newList[i].FirstName = columns[1];
                    newList[i].Street = columns[5]+" "+columns[6];
                    newList[i].ZipCode = Convert.ToInt32(columns[3]);
                    newList[i].City = columns[4];
                    newList[i].NumberOfInvitations=1;
                }
            }
            else
            {
                Console.WriteLine("Datei " + fileName + " nicht gefunden!");
            }

			return newList;
		}


        /// <summary>
		/// Zwei Listen mischen und Ergebnisliste zurückgeben
		/// </summary>
		/// <param name="listeA"></param>
		/// <param name="listeB"></param>
		/// <returns>Gemischte Ergebnisliste</returns>
		private static Friend[] MergeList(Friend[] invitationList, Friend[] newInvitations)
        {
            int actIdxNeu=0;
            int actIdxGesamt=0;
            bool found;

            if (newInvitations != null)
            {

                // Listen mischen, solange beide Listen noch Elemente enthalten
                while ((actIdxNeu<newInvitations.Length) && (newInvitations[actIdxNeu].LastName != null))
                {
                    //Prüfen ob aktueller Freund der neueListe bereits in gesamtListe vorhanden ist
                    found = false;
                    actIdxGesamt = 0;
                    while ((actIdxGesamt<invitationList.Length) && (invitationList[actIdxGesamt].LastName != null) && (!found))
                    {
                        if (invitationList[actIdxGesamt].LastName == newInvitations[actIdxNeu].LastName)
                        {
                            //Person bereits eingeladen
                            //Counter erhöhen
                            invitationList[actIdxGesamt].NumberOfInvitations++;
                            found = true;

                        }
                        actIdxGesamt++;
                    }
                    if (!found)
                    {
                        //Nicht gefunden -> neues Element anlegen
                        if (actIdxGesamt < invitationList.Length) //Sicherheitshabler prüfen, ob noch Platz in der Liste
                        {
                            invitationList[actIdxGesamt] = newInvitations[actIdxNeu];
                        }
                    }
                    actIdxNeu++;
                }
            }
            return invitationList;
        }


        /*
         * VARIANTE MIT VORSORTIERTEN LISTEN
         * 
		/// <summary>
		/// Zwei Listen mischen und Ergebnisliste zurückgeben
		/// </summary>
		/// <param name="listeA"></param>
		/// <param name="listeB"></param>
		/// <returns>Gemischte Ergebnisliste</returns>
		private static Freund[] MergeListe(Freund[] gesamtListe, Freund[] neueListe)
        {

			Freund[] ergebnisListe;

			int indexgesamtListe = 0;
			int indexNeueListe = 0;
			int indexErgebnisListe = 0;
			if (gesamtListe == null)
			{
				ergebnisListe = neueListe;
			}
			else if (neueListe == null)
			{
				ergebnisListe = gesamtListe;
			}
			else
			{
				ergebnisListe = new Freund[100];
				// Listen mischen, solange beide Listen noch Elemente enthalten
				while (gesamtListe[indexgesamtListe].NachName != null 
                    && neueListe[indexNeueListe].NachName != null)
				{
                    // a,b vom Typ string, DateTime, ... ==> vergleichbar (comparable)
                    // CompareTo(a,b) ==>  0, wenn a == b
                    //                     1, wenn a > b
                    //                    -1, wenn a < b  
					if (gesamtListe[indexgesamtListe].NachName.CompareTo(neueListe[indexNeueListe].NachName) < 0)
					{
						ergebnisListe[indexErgebnisListe] = gesamtListe[indexgesamtListe];
						indexgesamtListe++;
					}
					else if (gesamtListe[indexgesamtListe].NachName.CompareTo(neueListe[indexNeueListe].NachName) > 0)
					{
						ergebnisListe[indexErgebnisListe] = neueListe[indexNeueListe];
						indexNeueListe++;
					}
					else  // Namen sind gleich ==> nur einmal übernehmen
					{
						gesamtListe[indexErgebnisListe] = gesamtListe[indexgesamtListe];
						indexgesamtListe++;
						indexNeueListe++;
					}
					indexErgebnisListe++;
				}
				// Rest der verbleibenden Listen anhängen
				while (gesamtListe[indexgesamtListe].NachName != null)
				{
					gesamtListe[indexErgebnisListe] = gesamtListe[indexgesamtListe++];
                    indexErgebnisListe++;
				}
				while (neueListe[indexNeueListe].NachName != null)
				{
					gesamtListe[indexErgebnisListe++] = neueListe[indexNeueListe++];
				}
				ergebnisListe = gesamtListe;
                
			}
			return ergebnisListe;
		}
         
         */
    }
         

}
