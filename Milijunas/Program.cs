/*
|-------------------------------|
|                               |
| Autor: Mario Šomođi           |
| Projekt: Milijunaš            |
| Predmet: Osnove Programiranja |
| Ustanova: VSMTI               |
| Godina: 2019.                 |
|                               |
|-------------------------------|
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ConsoleTables;

namespace Milijunas
{
    public class Program
    {
        public struct Odgovori //Struktura pomocu koje znamo koji odgovor sadrzi koji sadrzaj
        {
            public string odgA;
            public string odgB;
            public string odgC;
            public string odgD;
            public Odgovori(string a, string b, string c, string d)
            {
                odgA = a;
                odgB = b;
                odgC = c;
                odgD = d;
            }
        }
        public struct Rezultat //Struktura pomocu koje spremimo sve rezultate iz xml-a
        {
            public string prvoImeIgraca;
            public string prezimeIgraca;
            public string datumIgranja;
            public int osvojeniIznos;
            public Rezultat(string ime, string prezime, string datumIgre, int dobitak)
            {
                prvoImeIgraca = ime;
                prezimeIgraca = prezime;
                datumIgranja = datumIgre;
                osvojeniIznos = dobitak;
            }
        }
        public struct Pitanje //Struktura pomocu koje spremimo sva pitanja iz xml-a
        {
            public string idPitanja;
            public string tezinaPitanja;
            public string upit;
            public string odgA;
            public string odgB;
            public string odgC;
            public string odgD;
            public string tocanOdg;
            public Pitanje(string id, string tezina, string pitanje, string a, string b, string c, string d, string tocno)
            {
                idPitanja = id;
                tezinaPitanja = tezina;
                upit = pitanje;
                odgA = a;
                odgB = b;
                odgC = c;
                odgD = d;
                tocanOdg = tocno;
            }
        }
        static int tocniOdgovori = 0;
        static bool tocno = true, odustaje = false;
        static List<Pitanje> listaPitanja = new List<Pitanje>();
        static List<Rezultat> rezultati = new List<Rezultat>();

        static void Main(string[] args)
        {
            OcistiLogDatoteku();
            LogajTrenutnuAkciju("Korisnik je pokrenuo program.");
            DohvatiIzbornik();
        }
        static void DohvatiIzbornik()
        {
            /*
            |---------------------------------------|
            |                                       |
            |Funkcija koja prikazuje glavni izbornik|
            |ovog projekta korisniku.               |
            |                                       |
            |---------------------------------------|
            */
            bool izlaz = false, prvoPokretanjeIzbornika = true;
            do
            {
                if (prvoPokretanjeIzbornika == false)
                {
                    PonovniPrikazIzbornikaIliIzlaz();
                }
                IspisiIzbornik();
                int odabir = OdabirOpcijeSaIzbornika();
                switch (odabir)
                {
                    case 1:
                        {
                            string prezimeIgraca, prvoImeIgraca;
                            Console.Clear(); //Ocisti sve linije u konzoli
                            LogajTrenutnuAkciju("Korisnik je odabrao opciju Nova igra.");
                            prvoImeIgraca = UnosTeProvjeraDaUnosNijePrazan("imena");
                            LogajTrenutnuAkciju("Korisnik je uspjesno unjeo svoje prvo ime.");
                            prezimeIgraca = UnosTeProvjeraDaUnosNijePrazan("prezimena");
                            Console.Clear();
                            DohvatiPitanja();
                            IspisPitanja();
                            int novcaniDobitak = ProvjeriPredeniPrag();
                            UpisRezultataUXML(prvoImeIgraca, prezimeIgraca, novcaniDobitak);
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            LogajTrenutnuAkciju("Korisnik je odabrao opciju Rezultati kandidata.");
                            DohvatiRezultate();
                            IspisiRezultate();
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            LogajTrenutnuAkciju("Korisnik je zatvorio program.");
                            izlaz = true;
                            OcistiMemoriju();
                            Environment.Exit(0); //Izlaz iz programa
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            LogajTrenutnuAkciju("Korisnik je odabrao nepostojecu opciju u izborniku, izbornik se ponovno otvara.");
                            Console.WriteLine("Unjeli ste nepostojecu opciju u izborniku!");
                            Console.WriteLine("Izbornik se ponovno otvara");
                            PauzirajProgram(1500);
                            Console.Clear();
                            DohvatiIzbornik();
                            break;
                        }
                }
                prvoPokretanjeIzbornika = false;
            } while (izlaz == false);
        }
        static void IspisiIzbornik()
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ispisuje izbornik.        |
            |                                        |
            |----------------------------------------|
            */
            Console.WriteLine("Izbornik\n1 - Nova igra\n2 - Rezultati kandidata\n3 - Izlaz");
        }
        static int OdabirOpcijeSaIzbornika()
        {
            /*
            |------------------------------------------|
            |                                          |
            |Funkcija koja dozvoljava samo unos brojeva|
            |sa tipkovnice.                            |
            |                                          |
            |------------------------------------------|
            */
            bool krajUnosa = false;
            Console.Write("Odabir: ");
            while (!krajUnosa)
            {
                ConsoleKeyInfo sadrzajUnosa = Console.ReadKey(true);
                char unos = sadrzajUnosa.KeyChar;

                if (char.IsDigit(unos))
                {
                    krajUnosa = true;
                    return (unos - '0');
                }
                else
                {
                    Console.WriteLine("Niste unjeli opciju koja postoji na izborniku");
                    System.Threading.Thread.Sleep(750);
                    OcistiOdredeniBrojLinijaUKonzoli(1);
                }
            }
            return 0;
        }
        static void DohvatiPitanja()
        {
            /*
            |-------------------------------------------|
            |                                           |
            |Funkcija koja ce sva pitanja iz pitanja.xml|
            |spremiti u listu tipa Pitanje.             |
            |                                           |
            |-------------------------------------------|
            */
            listaPitanja.Clear();
            string sadrzaj_xml = "";
            StreamReader oSr = new StreamReader("pitanja.xml");
            using (oSr)
            {
                sadrzaj_xml = oSr.ReadToEnd();
            }
            XmlDocument xml_datoteka = new XmlDocument();
            xml_datoteka.LoadXml(sadrzaj_xml);
            XmlNodeList atributi = xml_datoteka.SelectNodes("//data/pitanje");
            foreach (XmlNode vrijednost in atributi)
            {
                listaPitanja.Add(new Pitanje(vrijednost.Attributes["id"].Value, vrijednost.Attributes["tezina"].Value, vrijednost.Attributes["upit"].Value, vrijednost.Attributes["odgA"].Value, vrijednost.Attributes["odgB"].Value, vrijednost.Attributes["odgC"].Value, vrijednost.Attributes["odgD"].Value, vrijednost.Attributes["tocanOdg"].Value));
            }
            oSr.Close();
        }
        static void DohvatiRezultate()
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ce sve rezultate iz       |
            |rezultati_kandidata.xml spremiti u listu|
            |tipa rezultat                           |
            |                                        |
            |----------------------------------------|
            */
            rezultati.Clear();
            string sadrzaj_xml = "";
            StreamReader oSr = new StreamReader("rezultati_kandidata.xml");
            using (oSr)
            {
                sadrzaj_xml = oSr.ReadToEnd();
            }
            XmlDocument xml_datoteka = new XmlDocument();
            xml_datoteka.LoadXml(sadrzaj_xml);
            XmlNodeList atributi = xml_datoteka.SelectNodes("//data/rezultat");
            foreach (XmlNode vrijednost in atributi)
            {
                rezultati.Add(new Rezultat(vrijednost.Attributes["prvoImeIgraca"].Value, vrijednost.Attributes["prezimeIgraca"].Value, vrijednost.Attributes["datumIgranja"].Value, Convert.ToInt32(vrijednost.Attributes["osvojeniIznos"].Value)));
            }
            oSr.Close();
        }
        static int DohvatiUkupniBrojPitanja()
        {
            /*
            |-----------------------------------------|
            |                                         |
            |Funkcija koja ce prebrojati sva pitanja u|
            |pitanja.xml tako da znamo njihov ukupan  |
            |broj.                                    |
            |                                         |
            |-----------------------------------------|
            */
            int ukupnoPitanja = 0;
            foreach (var pitanje in listaPitanja)
            {
                ukupnoPitanja++;
            }
            return ukupnoPitanja += 1;
        }
        static void IspisPitanja()
        {
            /*
            |--------------------------------------------------|
            |                                                  |
            |Funkcija koja ce sva pitanja iz liste listaPitanja|
            |ispisati sljedno u slucaju tocnih odgovora.       |
            |                                                  |
            |--------------------------------------------------|
            */

            int brojPitanja = 1, odBrojaPitanja = 1, doBrojaPitanja = 4;
            string trenutnaTezina = "1";
            List<string> ids = new List<string>();
            Random rnd = new Random();
            int ukupnoPitanja = DohvatiUkupniBrojPitanja();
            while (brojPitanja < 16)
            {
                foreach (var pitanje in listaPitanja)
                {
                    if (brojPitanja >= odBrojaPitanja && brojPitanja <= doBrojaPitanja && pitanje.tezinaPitanja == trenutnaTezina)
                    {
                        string id = Convert.ToString(rnd.Next(1, ukupnoPitanja));
                        if (id == pitanje.idPitanja)
                        {
                            if (!ids.Contains(id))
                            {
                                Console.WriteLine(pitanje.upit);
                                var odgovori = RandomizirajIIspisiOdgovore(pitanje);
                                UnosOdg(pitanje, odgovori);
                                if (odustaje == true)
                                {
                                    goto kraj;
                                }
                                ids.Add(pitanje.idPitanja);
                                brojPitanja++;
                                Console.Clear();
                            }
                            switch (brojPitanja)
                            {
                                case 5:
                                    {
                                        trenutnaTezina = "2";
                                        odBrojaPitanja = 5;
                                        doBrojaPitanja = 6;
                                        break;
                                    }
                                case 7:
                                    {
                                        trenutnaTezina = "3";
                                        odBrojaPitanja = 7;
                                        doBrojaPitanja = 8;
                                        break;
                                    }
                                case 9:
                                    {
                                        trenutnaTezina = "4";
                                        odBrojaPitanja = 9;
                                        doBrojaPitanja = 10;
                                        break;
                                    }
                                case 11:
                                    {
                                        trenutnaTezina = "5";
                                        odBrojaPitanja = 11;
                                        doBrojaPitanja = 12;
                                        break;
                                    }
                                case 13:
                                    {
                                        trenutnaTezina = "6";
                                        odBrojaPitanja = 13;
                                        doBrojaPitanja = 14;
                                        break;
                                    }
                                case 15:
                                    {
                                        trenutnaTezina = "7";
                                        odBrojaPitanja = 15;
                                        doBrojaPitanja = 15;
                                        break;
                                    }
                            }
                        }
                    }
                    if (tocno == false)
                    {
                        goto kraj;
                    }
                }
            }
        kraj:;
        }
        static void IspisiRezultate()
        {
            /*
            |--------------------------------------------------|
            |                                                  |
            |Funkcija koja ce sve rezultate iz liste rezultati |
            |ispisati u tablici od najveceg osvojenog          |
            |novcanog iznosa do najmanjeg.                     |
            |                                                  |
            |--------------------------------------------------|
            */
            int redniBroj = 0;
            double porez = 0;
            rezultati = rezultati.OrderByDescending(x => x.osvojeniIznos).ToList();
            var table = new ConsoleTable("Redni broj", "Ime", "Prezime", "Datum pristupa", "Osvojeni iznos", "Porez na osvojeni iznos");
            foreach (Rezultat rezultatkandidata in rezultati)
            {
                double dobitak = Convert.ToInt32(rezultatkandidata.osvojeniIznos);
                if (dobitak < 30000)
                {
                    porez = dobitak * 0.35;
                }
                else 
                {
                    porez = dobitak * 0.45;
                }
                table.AddRow(++redniBroj + ".", rezultatkandidata.prvoImeIgraca, rezultatkandidata.prezimeIgraca, rezultatkandidata.datumIgranja, rezultatkandidata.osvojeniIznos + "kn", porez);
                porez = 0;
            }
            table.Write();
            OcistiOdredeniBrojLinijaUKonzoli(1);
        }
        static Odgovori RandomizirajIIspisiOdgovore(Pitanje pitanje)
        {
            /*
            |-----------------------------------------------|
            |                                               |
            |Funkcija koja ce iz pitanja.xml random ispisati|
            |odgovore na odredeno pitanje tako da njihov    |
            |redosljen nebude isti svaku igru.              |
            |                                               |
            |-----------------------------------------------|
            */
            string sadrzajOdgovoraA = "", sadrzajOdgovoraB = "", sadrzajOdgovoraC = "", sadrzajOdgovoraD = "", abcd = "a", sadrzajOdgovora = "";
            int brojOdg = 1;
            List<int> iskoristeniOdgovori = new List<int>();
            while (brojOdg < 5)
            {
                Random rnd = new Random();
                int odg = rnd.Next(1, 5);
                switch (odg)
                {
                    case 1:
                        {
                            sadrzajOdgovora = pitanje.odgA;
                            break;
                        }
                    case 2:
                        {
                            sadrzajOdgovora = pitanje.odgB;
                            break;
                        }
                    case 3:
                        {
                            sadrzajOdgovora = pitanje.odgC;
                            break;
                        }
                    case 4:
                        {
                            sadrzajOdgovora = pitanje.odgD;
                            break;
                        }
                }

                if (!iskoristeniOdgovori.Contains(odg))
                {
                    Console.WriteLine("{0}) {1}", abcd, sadrzajOdgovora);
                    iskoristeniOdgovori.Add(odg);
                    switch (brojOdg)
                    {
                        case 1:
                            {
                                sadrzajOdgovoraA = sadrzajOdgovora;
                                abcd = "b";
                                break;
                            }
                        case 2:
                            {
                                sadrzajOdgovoraB = sadrzajOdgovora;
                                abcd = "c";
                                break;
                            }
                        case 3:
                            {
                                sadrzajOdgovoraC = sadrzajOdgovora;
                                abcd = "d";
                                break;
                            }
                        case 4:
                            {
                                sadrzajOdgovoraD = sadrzajOdgovora;
                                break;
                            }
                    }
                    brojOdg++;
                }
            }
            Console.WriteLine("Ako zelis odustati napisi Da");
            Odgovori sadrzajiSvihOdgovora = new Odgovori(sadrzajOdgovoraA, sadrzajOdgovoraB, sadrzajOdgovoraC, sadrzajOdgovoraD);
            return sadrzajiSvihOdgovora;
        }
        static void UnosOdg(Pitanje pitanje, Odgovori odgovori)
        {
        /*
        |---------------------------------------------|
        |                                             |
        |Funkcija koja provjerava jeli odgovor koji je|
        |korisnik unjeo tocan ili netocan.            |
        |                                             |
        |---------------------------------------------|
        */
        nastavak:
            string odgovor = "";
            while (odgovor.ToLower() != "a" && odgovor.ToLower() != "b" && odgovor.ToLower() != "c" && odgovor.ToLower() != "d" && odgovor.ToLower() != "da")
            {
                Console.Write("Unesite vas odgovor (a, b, c ili d): ");
                odgovor = Console.ReadLine();
                if (odgovor.ToLower() != "a" && odgovor.ToLower() != "b" && odgovor.ToLower() != "c" && odgovor.ToLower() != "d" && odgovor.ToLower() != "da")
                {
                    Console.WriteLine("Niste unjeli niti jedan od ponudenih odgovora niti odlucili odustati!");
                    Console.WriteLine("Polje za unos odgovora se ponovno otvara.");
                    LogajTrenutnuAkciju("Korisnik nije unjeo niti jedan od odgovora niti odlucio odustati polje za unos odgovora se ponovno otvara.");
                    System.Threading.Thread.Sleep(2500);
                    OcistiOdredeniBrojLinijaUKonzoli(3);
                }
            }
            if (odgovor.ToLower() == "a")
            {
                odgovor = odgovori.odgA;
            }
            else if (odgovor.ToLower() == "b")
            {
                odgovor = odgovori.odgB;
            }
            else if (odgovor.ToLower() == "c")
            {
                odgovor = odgovori.odgC;
            }
            else if (odgovor.ToLower() == "d")
            {
                odgovor = odgovori.odgD;
            }
            if (odgovor.ToLower() == "da")
            {
                string siguran = "";
                while (siguran.ToLower() != "da" && siguran.ToLower() != "ne")
                {
                    Console.Write("Jeste li sigurni: ");
                    siguran = Console.ReadLine();
                    if (siguran.ToLower() != "da" && siguran.ToLower() != "ne")
                    {
                        Console.WriteLine("Niste unjeli niti da niti ne, unos se ponovno otvara.");
                        System.Threading.Thread.Sleep(1000);
                        OcistiOdredeniBrojLinijaUKonzoli(2);
                    }
                }
                if (siguran.ToLower() == "da")
                {
                    odustaje = true;
                    Console.WriteLine("Odlucili ste odustati.");
                    LogajTrenutnuAkciju("Korisnik je odlucio odustati.");
                    System.Threading.Thread.Sleep(750);
                    Console.Clear();
                }
                else
                {
                    System.Threading.Thread.Sleep(750);
                    OcistiOdredeniBrojLinijaUKonzoli(3);
                    goto nastavak;
                }
            }
            if (odustaje == false && odgovor.ToLower() != "da")
            {
                string konacanOdgovor = "";
                while (konacanOdgovor.ToLower() != "da" && konacanOdgovor.ToLower() != "ne")
                {
                    Console.Write("Jeli to vas konacan odgovor (Da, Ne): ");
                    konacanOdgovor = Console.ReadLine();
                    if (konacanOdgovor.ToLower() != "da" && konacanOdgovor.ToLower() != "ne")
                    {
                        Console.WriteLine("Niste unjeli niti jedan od ponudenih odabira!");
                        Console.WriteLine("Polje za odluku o konacnom odgovoru se ponovno otvara.");
                        LogajTrenutnuAkciju("Korisnik nije unjeo niti jedan od ponudenih odabira polje za odluku o konacnom odgovoru se ponovno otvara.");
                        System.Threading.Thread.Sleep(2500);
                        OcistiOdredeniBrojLinijaUKonzoli(3);
                    }
                }
                if (konacanOdgovor.ToLower() == "da")
                {
                    if (odgovor == pitanje.tocanOdg)
                    {
                        LogajTrenutnuAkciju("Korisnik je unjeo tocan odgovor.");
                        Console.WriteLine("TOCNO!");
                        System.Threading.Thread.Sleep(500);
                        tocno = true;
                        tocniOdgovori++;
                    }
                    else
                    {
                        LogajTrenutnuAkciju("Korisnik je unjeo netocan odgovor.");
                        tocno = false;
                        Console.WriteLine("NETOCNO!");
                        System.Threading.Thread.Sleep(500);
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                    OcistiOdredeniBrojLinijaUKonzoli(2);
                    UnosOdg(pitanje, odgovori);
                }
            }
        }
        static int ProvjeriPredeniPrag()
        {
            /*
            |--------------------------------------------------=|
            |                                                   |
            |Funkcija koje ce provjeriti jesmo li presli koji od|
            |pragova te vratiti nas dobiveni novcani iznos ako  |
            |jesmo te ako nismo vratiti da nismo nista osvojili.|
            |                                                   |
            |---------------------------------------------------|
            */
            int novcaniDobitak;

            if (tocniOdgovori >= 5 && tocniOdgovori < 10)
            {
                Console.WriteLine("Odgovorili ste tocno na {0}. pitanja i presli prvi prag te nosite kuci 1000kn", tocniOdgovori);
                novcaniDobitak = 1000;
            }
            else if (tocniOdgovori >= 10 && tocniOdgovori < 15)
            {
                Console.WriteLine("Odgovorili ste tocno na {0}. pitanja i presli drugi prag te nosite kuci 32000kn", tocniOdgovori);
                novcaniDobitak = 32000;
            }
            else if (tocniOdgovori == 15)
            {
                Console.WriteLine("Cestitam, odgovorili ste tocno na sva pitanja!!!\nNosite kuci 1 MILIJUN kuna i titulu Milijunasa!");
                novcaniDobitak = 1000000;
            }
            else
            {
                Console.WriteLine("Niste presli niti prvi prag tako da nazalost ovaj puta niste nista osvojili.");
                novcaniDobitak = 0;
            }
            return novcaniDobitak;
        }
        static void UpisRezultataUXML(string ime, string prezime, int dobitak)
        {
            /*
            |------------------------------------------------------|
            |                                                      |
            |Funkcija kojom upisujemo rezultate korsinika u xml    |
            |datoteku.                                             |
            |                                                      |
            |------------------------------------------------------|
            */
            bool postoji = File.Exists("rezultati_kandidata.xml");
            if (postoji == false)
            {
                DateTime now = DateTime.Now;
                XDocument doc = new XDocument(
                new XDeclaration("1.0", "gb2312", string.Empty),
                new XElement("data",
                    new XElement("rezultat",
                        new XAttribute("prvoImeIgraca", ime),
                        new XAttribute("prezimeIgraca", prezime),
                        new XAttribute("datumIgranja", now.ToString("d")),
                        new XAttribute("osvojeniIznos", Convert.ToString(dobitak))
                        )));
                doc.Save("rezultati_kandidata.xml");
            }
            else
            {
                DateTime now = DateTime.Now;
                var doc = XDocument.Load("rezultati_kandidata.xml");
                var newElement = new XElement("rezultat",
                                    new XAttribute("prvoImeIgraca", ime),
                                    new XAttribute("prezimeIgraca", prezime),
                                    new XAttribute("datumIgranja", now.ToString("d")),
                                    new XAttribute("osvojeniIznos", Convert.ToString(dobitak))
                                );
                doc.Element("data").Add(newElement);
                doc.Save("rezultati_kandidata.xml");
            }
        }
        static void PonovniPrikazIzbornikaIliIzlaz()
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ponovno ispisuje izbornik |
            |nakon svake akcije u programu.          |
            |                                        |
            |----------------------------------------|
            */
            Console.WriteLine("Pritisnite tipku <Enter> za ponovni prikaz izbornika, a tipku <ESC> za izlaz iz programa");
            bool krajUnosa = false;
            tocniOdgovori = 0;
            tocno = true;
            odustaje = false;
            while (!krajUnosa)
            {
                ConsoleKeyInfo sadrzajUnosa = Console.ReadKey(true);

                if (sadrzajUnosa.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    krajUnosa = true;
                    LogajTrenutnuAkciju("Korisnik je odlucio ponovno prikazati izbornik.");
                }
                else if (sadrzajUnosa.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    LogajTrenutnuAkciju("Korisnik je zatvorio program.");
                    OcistiMemoriju();
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Niste pritisnuli niti jednu od ponudenih tipki.");
                    System.Threading.Thread.Sleep(750);
                    OcistiOdredeniBrojLinijaUKonzoli(1);
                }
            }
        }
        static void IspisUnosa(string polje)
        {
            /*
            |--------------------------------------------|
            |                                            |
            |Funkcija koja ispisuje ime trenutnog podatka|
            |kojeg upisujemo.                            |
            |                                            |
            |--------------------------------------------|
            */
            Console.Write("Unesi svoje {0} : ", polje);
        }
        static string UnosTeProvjeraDaUnosNijePrazan(string polje)
        {
            /*
            |------------------------------------------------|
            |                                                |
            |Funkcija koja ce korisniku dati mogucnost unosa |
            |odredenog podatka te provjeriti da korisnik nije|
            |ostavio polje za unos tog podatka praznim.      |
            |                                                |
            |------------------------------------------------|
            */
            string sadrzajUnosa = "";
            while (sadrzajUnosa == "")
            {
                IspisUnosa(polje);
                sadrzajUnosa = Console.ReadLine();
                if (sadrzajUnosa == "")
                {
                    IspisiError(polje, 3);
                }
            }
            return sadrzajUnosa;
        }
        static void IspisiError(string polje, int brojLinija)
        {
            /*
            |------------------------------------------------|
            |                                                |
            |Funkcija koja ispisuje error ako je polje za    |
            |upis odredenog podatka ostavljeno praznim.      |
            |                                                |
            |------------------------------------------------|
            */
            Console.WriteLine("Nepravilni unos u polje za unos {0}", polje);
            Console.WriteLine("Polja za unos {0} se ponovno otvara.", polje);
            LogajTrenutnuAkciju("Korisnik je ostavio polje za unos " + polje + " praznim te se polje ponovno otvara.");
            PauzirajProgram(1500);
            OcistiOdredeniBrojLinijaUKonzoli(brojLinija);
        }
        static void PauzirajProgram(int trajanje)
        {
            /*
            |------------------------------------------------|
            |                                                |
            |Funkcija koja pauzira program.                  |
            |                                                |
            |------------------------------------------------|
            */
            System.Threading.Thread.Sleep(trajanje);
        }


        static void OcistiLinijuUKonzoli()
        {
            /*
            |----------------------------------------------|
            |                                              |
            |Funkcija koja koja postavi kursor na prijasnju|
            |liniju u konzoli i napisi prazni string u tu  |
            |liniju tj ocisti tu liniju u konzoli.         |    
            |                                              |
            |----------------------------------------------|
            */
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        static void OcistiOdredeniBrojLinijaUKonzoli(int brojLinija)
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ce ocistiti zadaani broj  |
            |linija u konzoli koji joj mi posaljemo  |
            |                                        |
            |----------------------------------------|
            */
            int brojac = 0;
            while (brojac < brojLinija)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                OcistiLinijuUKonzoli();
                brojac++;
            }
        }
        static void OcistiLogDatoteku()
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ocisti logs.log datoteku  |
            |pri svakom pokretanju programa.         |
            |                                        |
            |----------------------------------------|
            */
            StreamWriter log = new StreamWriter("logs.log", false);
            log.Flush();
            log.Close();
        }
        static void LogajTrenutnuAkciju(string akcija)
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja upise akciju koju je      |
            |korisnik napravio u logs.log datoteku.  |
            |                                        |
            |----------------------------------------|
            */
            DateTime now = DateTime.Now;
            StreamWriter log = new StreamWriter("logs.log", true);
            log.WriteLine("{0} - {1}", now.ToString("F"), akcija);
            log.Flush();
            log.Close();
        }
        static void OcistiMemoriju() 
        {
            /*
            |----------------------------------------|
            |                                        |
            |Funkcija koja ce ocistiti memoriju pri  |
            |zatvaranju programa.                    |
            |                                        |
            |----------------------------------------|
            */
            listaPitanja.Clear();
            rezultati.Clear();
        }
    }
}