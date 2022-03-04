using System;
using System.Collections.Concurrent;

namespace MinotaurVase
{
    internal class Program
    {
        /// <summary>
        /// The only viable strategy for this situation realistically
        /// is the third strategy.
        /// 
        /// This is because the first strategy would, as the problem statement
        /// said "cause large crowds of eager guests to gather around the door"
        /// This would not only be pandemonium but also not guarantee that any 
        /// individual guest would be allowed to enter the room.
        /// 
        /// The second strategy is slightly better than the first in that it would
        /// result in less guests crowding the door to the vase, however, this
        /// strategy would also result in some guests being unlucky and not being 
        /// able to see the vase. When attempting to do it this way, it resulted in
        /// many guests going into the showroom over and over, not allowing any other 
        /// guests the ability to enter.
        /// 
        /// Thus, by creating a queue for the guests, it would allow any guest
        /// that is in the queue the ability to see the vase eventually, going in
        /// as they see the previous guest leave through the single door.
        /// </summary>


        // N = 10 by default
        private static int guests = 10;
        // Determines how long a guest spends in the showroom
        // Also determins the time between guests considering to enqueue
        private static int admireTime = 500;
        // Determines how long the party lasts
        private static int partyTime = 10000;
        // Determines if the showroom is occupied, default is yes because the minotaur
        // will wait until all guests arrive before opening it.
        private static bool occupied = true;
        // Determins the percent chance that a guest enqueues themselves
        private static int percent = 40;
        // Is the list of guests in the queue
        private static ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        // For randomly generating the guests opinions on entering the queue
        private static Random rand = new Random();
        static void Main(string[] args)
        {
            // Initializing variables
            Thread[] threads = new Thread[guests];

            // Runs the loop where the minotaur randomly selects a guest to go into the labyrinth
            for(int i = 0; i < guests; i++)
            {
                int tI = i;
                threads[i] = new Thread(new ThreadStart(() => Party(tI)));
                threads[i].Start();
            }

            // The minotaur waits for all guests to arrive before opening the vase, to be fair
            occupied = false;
            // The party will run for a preset amount of time before
            Thread.Sleep(partyTime);

            // The code will exit the loop when a guest has filled up the last wrapper pile, thus meaning that every guest has entered.
            Console.WriteLine("The party has concluded, thank you for enjoying my vase");
            Console.WriteLine("\t\t-Minotaur");
            //printQueue();
            Environment.Exit(0);

        }
        // Simulates a guest in the party
        static void Party(int i)
        {
            int r, item;
            while (true)
            {
                // Every admireTime, the guest has a chance to enqueue themselves
                r = rand.Next(100);
                if(r < percent)
                {
                    queue.Enqueue(i);
                    while(true)
                    {
                        // While in the queue, they wait for the room to not be occupied
                        if(!occupied)
                        {
                            queue.TryPeek(out item);
                            // Then it verifies that they are at the front of the line and the room is still not occupied
                            if(item == i && !occupied)
                            {
                                // Once this occurs they enter the room and when they leave they return back to the party
                                Showroom(i);
                                break;
                            }
                        }
                    }
                }
                // There is a customizable time gap between considering entering the queue
                Thread.Sleep(admireTime);
            }
        }
        // Simulates a guest running the labyrinth
        static void Showroom(int i)
        {
            // First, it sets the showroom as occupied and removes the
            // guest from the queue, and then announces that they have entered
            occupied = true;
            queue.TryDequeue(out i);
            Console.WriteLine($"Guest {i} entering the showroom");

            // The guest waits for their designated amount of admiration time
            Thread.Sleep(admireTime);

            // The guest then leaves to continue about their time at the party
            Console.WriteLine($"Guest {i} has left the showroom");
            occupied = false;
            //printQueue();
        }

        // Prints the entire queue, used for testing
        static void printQueue()
        {
            foreach (var guest in queue)
            {
                Console.Write(guest + ", ");
            }
            Console.Write("\n");
        }
    }
}