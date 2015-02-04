using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Fleet
{
    public enum OrganismHull {CORVETTE, FRIGATE, CRUISER, BATTLESHIP, COUNT}
    public enum OrganismArchetype {SNIPER, TANK, SUPPORT, DPS, COUNT}

    struct SlotsPerSection
    {
        public int forward;
        public int aft;
        public int port;
        public int starboard;
    }

    class Organism
    {
        private SlotsPerSection slots;
        public AI_Fleet.SlotsPerSection Slots
        {
            get { return slots; }
            set { slots = value; }
        }

        private List<Chromosome> genome;
        public List<Chromosome> Genome
        {
            get { return genome; }
            set { genome = value; }
        }

        private static uint numOrganisms;
        public uint ID;

        private OrganismHull hull;
        public AI_Fleet.OrganismHull Hull
        {
            get { return hull; }
            set { hull = value; }
        }

        private OrganismArchetype archetype;
        public AI_Fleet.OrganismArchetype Archetype
        {
            get { return archetype; }
            set { archetype = value; }
        }

        public Organism()
        {
            initOrganism();
            //assign a completely random genome
            hull = (OrganismHull)(RandomManager.rollDwhatever((int)OrganismHull.COUNT));
            archetype = (OrganismArchetype)(RandomManager.rollDwhatever((int)OrganismArchetype.COUNT));
            Console.WriteLine(hull);
            Console.WriteLine(archetype);
            genome = new List<Chromosome>();
            SlotsPerSection remainingSlots = slots;
            int numGenes = 0;

            switch (hull)
            {
                case OrganismHull.CORVETTE:
                    // 5 - 10 Genes
                    numGenes = RandomManager.randomInt(20,30);
                    break;
                case OrganismHull.FRIGATE:
                    // 7 - 14 Genes
                    numGenes = RandomManager.randomInt(30, 50);
                    break;
                case OrganismHull.CRUISER:
                    //9 - 18 Genes
                    numGenes = RandomManager.randomInt(40, 70);
                    break;
                case OrganismHull.BATTLESHIP:
                    //12 - 23 Genes
                    numGenes = RandomManager.randomInt(60, 91);
                    break;
                default:
                    break;
            }
            Gene gene;

            for (int i = 0; i < 5; i++)
            {
                genome.Add(new Chromosome());
                genome[i].Alleles = new List<Gene>();

                int numTries;
                bool found;

                for (int j = 0; j < numGenes; j++)
                {
                    numTries = 0;
                    found = false;
                    remainingSlots = slots;
                    do
                    {
                        gene = new Gene(hull);

                        switch (gene.Placement)
                        {
                            case PlacementType.FORWARD:
                                if (gene.Count < remainingSlots.forward)
                                {
                                    found = true;
                                    remainingSlots.forward -= gene.Count;
                                }
                                break;
                            case PlacementType.AFT:
                                if (gene.Count < remainingSlots.aft)
                                {
                                    found = true;
                                    remainingSlots.aft -= gene.Count;
                                }
                                break;
                            case PlacementType.PORT:
                                if (gene.Count < remainingSlots.port)
                                {
                                    found = true;
                                    remainingSlots.port -= gene.Count;
                                }
                                break;
                            case PlacementType.STARBOARD:
                                if (gene.Count < remainingSlots.starboard)
                                {
                                    found = true;
                                    remainingSlots.starboard -= gene.Count;
                                }
                                break;
                            default:
                                break;
                        }

                        numTries++;

                    } while (!found && numTries > 200);

                    if (found)
                    {
                        genome[i].Alleles.Add(gene);
                    }
                    
                }
            }



            
        }

        public Organism(List<Chromosome> _genome, OrganismHull _hull = OrganismHull.CORVETTE, OrganismArchetype _archetype = OrganismArchetype.SNIPER)
        {
            hull = _hull;
            archetype = _archetype;
            genome = _genome;
            initOrganism();
        }

        private void initOrganism()
        {
            incrementIDs();
            assignSlots();
        }

        private void incrementIDs()
        {
            ID = ++numOrganisms;
        }

        private void assignSlots()
        {
            switch (hull)
            {
                case OrganismHull.CORVETTE:
                    slots.forward = 9;
                    slots.aft = 9;
                    slots.port = 7;
                    slots.starboard = 6;
                    break;
                case OrganismHull.FRIGATE:
                    slots.forward = 9;
                    slots.aft = 11;
                    slots.port =14;
                    slots.starboard =14;
                    break;
                case OrganismHull.CRUISER:
                    slots.forward = 21;
                    slots.aft = 21;
                    slots.port = 14;
                    slots.starboard = 14;
                    break;
                case OrganismHull.BATTLESHIP:
                    slots.forward = 27;
                    slots.aft = 28;
                    slots.port = 18;
                    slots.starboard = 18;
                    break;
                default:
                    break;
            }
        }

        private ShipBluePrint BluePrintFromChromosomes(List<Chromosome> _genome)
        {
            return new ShipBluePrint();
        }

        public void DebugDisplay()
        {
            Console.WriteLine("Organism ID: " + ID + " hull: " + hull + " archetype: " + archetype);
            Console.WriteLine("____________________________________________________________________");
            foreach (Chromosome chromosome in genome)
	        {
                chromosome.DebugDisplay();
	        }
            Console.WriteLine("____________________________________________________________________");
        }

        
    }
}