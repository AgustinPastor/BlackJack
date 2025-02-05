
namespace Name
{
    using System;
using System.Collections.Generic;

public class Carta
{
    public string Valor { get; }
    public string Palo { get; }

    public Carta(string valor, string palo)
    {
        Valor = valor;
        Palo = palo;
    }
}

public class Baraja
{
    private List<Carta> cartas;
    private Random random;

    public Baraja()
    {
        random = new Random();
        cartas = new List<Carta>();
        string[] palos = { "Corazones", "Tréboles", "Picas", "Diamantes" };
        string[] valores = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        foreach (var palo in palos)
        {
            foreach (var valor in valores)
            {
                cartas.Add(new Carta(valor, palo));
            }
        }
        Barajar();
    }

    private void Barajar()
    {
        for (int i = cartas.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cartas[i], cartas[j]) = (cartas[j], cartas[i]);
        }
    }

    public Carta RobarCarta()
    {
        if (cartas.Count == 0)
        {
            Console.WriteLine("Se han agotado las cartas. Barajando de nuevo...");
            Barajar();
        }

        Carta cartaRobada = cartas[^1];
        cartas.RemoveAt(cartas.Count - 1);
        return cartaRobada;
    }
}

public class Mano
{
    public List<Carta> Cartas { get; }

    public Mano()
    {
        Cartas = new List<Carta>();
    }

    public void AgregarCarta(Carta carta)
    {
        Cartas.Add(carta);
    }

    public int CalcularPuntuacion()
    {
        int puntuacion = 0;
        int ases = 0;

        foreach (var carta in Cartas)
        {
            if (int.TryParse(carta.Valor, out int numero))
            {
                puntuacion += numero;
            }
            else if (carta.Valor == "A")
            {
                ases++;
                puntuacion += 1; // Primero contamos el As como 1
            }
            else
            {
                puntuacion += 10; // J, Q, K valen 10
            }
        }

        // Convertimos ases de 1 a 11 si no nos pasamos de 21
        while (ases > 0 && puntuacion + 10 <= 21)
        {
            puntuacion += 10;
            ases--;
        }

        return puntuacion;
    }

    public void MostrarMano(string nombre, bool ocultarPrimeraCarta = false)
    {
        Console.WriteLine($"{nombre} tiene las siguientes cartas:");
        for (int i = 0; i < Cartas.Count; i++)
        {
            if (ocultarPrimeraCarta && i == 0)
            {
                Console.WriteLine("[Carta oculta]");
            }
            else
            {
                Console.WriteLine($"{Cartas[i].Valor} de {Cartas[i].Palo}");
            }
        }

        if (!ocultarPrimeraCarta)
        {
            Console.WriteLine($"Puntuación: {CalcularPuntuacion()}");
        }
        Console.WriteLine();
    }
}

public class Juego
{
    private Baraja baraja;
    private Mano manoJugador;
    private Mano manoBanca;

    public Juego()
    {
        baraja = new Baraja();
        manoJugador = new Mano();
        manoBanca = new Mano();
    }

    public void Jugar()
    {
        while (true)
        {
            Console.Clear();
            manoJugador = new Mano();
            manoBanca = new Mano();

            // Repartir cartas iniciales
            manoJugador.AgregarCarta(baraja.RobarCarta());
            manoJugador.AgregarCarta(baraja.RobarCarta());
            manoBanca.AgregarCarta(baraja.RobarCarta());
            manoBanca.AgregarCarta(baraja.RobarCarta());

            // Mostrar manos
            manoJugador.MostrarMano("Jugador");
            manoBanca.MostrarMano("Banca", ocultarPrimeraCarta: true);

            // Turno del jugador
            while (manoJugador.CalcularPuntuacion() < 21)
            {
                Console.WriteLine("¿Quieres pedir otra carta? (s/n)");
                string decision = Console.ReadLine()?.ToLower();
                if (decision == "s")
                {
                    manoJugador.AgregarCarta(baraja.RobarCarta());
                    manoJugador.MostrarMano("Jugador");
                }
                else
                {
                    break;
                }
            }

            if (manoJugador.CalcularPuntuacion() > 21)
            {
                Console.WriteLine("Te has pasado de 21. ¡Pierdes!");
                MostrarResultados();
                return;
            }

            // Turno de la banca
            while (manoBanca.CalcularPuntuacion() < 17)
            {
                manoBanca.AgregarCarta(baraja.RobarCarta());
            }

            MostrarResultados();

            // Preguntar si el jugador quiere jugar otra vez
            Console.WriteLine("¿Quieres jugar otra ronda? (s/n)");
            if (Console.ReadLine()?.ToLower() != "s")
            {
                break;
            }
        }
    }

    private void MostrarResultados()
    {
        Console.Clear();
        manoJugador.MostrarMano("Jugador");
        manoBanca.MostrarMano("Banca");

        int puntuacionJugador = manoJugador.CalcularPuntuacion();
        int puntuacionBanca = manoBanca.CalcularPuntuacion();

        if (puntuacionJugador > 21)
        {
            Console.WriteLine("Te has pasado de 21. La banca gana.");
        }
        else if (puntuacionBanca > 21 || puntuacionJugador > puntuacionBanca)
        {
            Console.WriteLine("¡Felicidades, has ganado!");
        }
        else if (puntuacionJugador < puntuacionBanca)
        {
            Console.WriteLine("La banca gana. ¡Mejor suerte la próxima vez!");
        }
        else
        {
            Console.WriteLine("Empate.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        while (true)  // Bucle infinito hasta que el jugador decida salir
        {
            Juego juego = new Juego();
            juego.Jugar();

            Console.WriteLine("¿Quieres jugar otra ronda? (s/n)");
            string respuesta = Console.ReadLine()?.ToLower();

            if (respuesta != "s")
            {
                Console.WriteLine("Gracias por jugar. ¡Hasta la próxima!");
                break;  // Sale del bucle y termina el programa
            }
        }
    }
}
}
