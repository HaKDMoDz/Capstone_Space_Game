using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace GeneticAlgorithm
{
    public class Chromosome
    {
        private readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_=+[{]};:,<.>/?`~!@#$%^&*()";

        private string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        // The genetic info needed for this organism to survive natural selection
        // in this case genetic information will be technology levels, weapon choices, and quantified tactics
        //each "Generation" the genetic info will be tested against an ideal organism and either bred or killed
        
        //for testing purposes to make sure the selection process is working, etc
        private List<string> testGenetics = new List<string>();
        public List<string> TestGenetics
        {
            get { return testGenetics; }
            set { testGenetics = value; }
        }

        public void RandomizeGenome()
        {
            testGenetics.RemoveRange(0, 1);
            testGenetics.Add(Path.GetRandomFileName());

        }

        public Chromosome()
        {
            testGenetics.Add(Path.GetRandomFileName().Replace(".", ""));
        }
    }
}
/*
    class Chromosome
    {
        private static int GENOME_SIZE = 64;
        private ulong genes;
        private static GENE[] allGenes = new GENE[]{
            GENE.GENE_00, GENE.GENE_01, GENE.GENE_02, GENE.GENE_03, GENE.GENE_04, GENE.GENE_05, GENE.GENE_06, GENE.GENE_07, 
            GENE.GENE_08, GENE.GENE_09, GENE.GENE_10, GENE.GENE_11, GENE.GENE_12, GENE.GENE_13, GENE.GENE_14, GENE.GENE_15, 
            GENE.GENE_16, GENE.GENE_17, GENE.GENE_18, GENE.GENE_19, GENE.GENE_20, GENE.GENE_21, GENE.GENE_22, GENE.GENE_23,
            GENE.GENE_24, GENE.GENE_25, GENE.GENE_26, GENE.GENE_27, GENE.GENE_28, GENE.GENE_29, GENE.GENE_30, GENE.GENE_31,
            GENE.GENE_32, GENE.GENE_33, GENE.GENE_34, GENE.GENE_35, GENE.GENE_36, GENE.GENE_37, GENE.GENE_38, GENE.GENE_39,
            GENE.GENE_40, GENE.GENE_41, GENE.GENE_42, GENE.GENE_43, GENE.GENE_44, GENE.GENE_45, GENE.GENE_46, GENE.GENE_47, 
            GENE.GENE_48, GENE.GENE_49, GENE.GENE_50, GENE.GENE_51, GENE.GENE_52, GENE.GENE_53, GENE.GENE_54, GENE.GENE_55, 
            GENE.GENE_56, GENE.GENE_57, GENE.GENE_58, GENE.GENE_59, GENE.GENE_60, GENE.GENE_61, GENE.GENE_62, GENE.GENE_63};

        private static Random _random = new Random();
        public Guid Id { get; set; }

        private int numGenes; public int NumGenes { get { return numGenes; } set { numGenes = value; } }

        public bool isGeneSet(GENE _gene)
        {
            if ((genes & (ulong)_gene) > 0) // bitwise AND genes and the gene you are looking for, if the result is > 0 that gene is present
            {
                return true;
            }
            return false;
        }

        private void SetGene(GENE _gene)
        {
            genes |= (ulong)_gene; // bitwise OR
            numGenes++;
        }

        private void UnSetGene(GENE _gene)
        {
            genes &= (~(ulong)_gene); // bitwise AND
            numGenes--;
        }

        public ulong getGenome()
        {
            return genes;
        }

        public Chromosome()
        {
            genes = (ulong)0;
        }

        public Chromosome(ulong _dna)
        {
            genes = _dna;
        }

        public int GetTotal(Chromosome ideal)
        {
            int numMatching = 0;
            for (int i = 0; i < Chromosome.GENOME_SIZE - 1; i++)
            {
                if (isGeneSet(allGenes[i]) && ideal.isGeneSet(allGenes[i]))
                {
                    Console.Out.WriteLine("found a matching gene");
                    numMatching++;
                }
            }
            return numMatching;
        }
        
        public void RandomizeGenome()
        {
            //sets between 1 and GENOME_SIZE random genes
            for (int i = 0; i < _random.Next(GENOME_SIZE + 1); i++)
            {
                SetGene(allGenes[_random.Next(GENOME_SIZE)]);
                numGenes++;
            }
        }

        public void SwapWith(Chromosome _chromosome, int toPosition)
        {
            ulong fromMask = 0x1111111111111111;
            ulong toMask = 0x1111111111111111;

            for (int i = 0; i < toPosition; i++)
            {
                fromMask = fromMask >> 1;
            }

            for (int i = toPosition; i < _chromosome.NumGenes - 1; i++)
            {
                toMask = toMask << 1;
            }

            ulong firstGenes = genes & fromMask;
            ulong secondGenes = _chromosome.genes & toMask;

            ulong newGenes = firstGenes | secondGenes;
            genes = newGenes;
        }

        public void SwapGenes(int position1, int position2)
        {
            GENE firstGene = allGenes[position1];
            GENE secondGene = allGenes[position2];

            if (isGeneSet(firstGene))
            {
                UnSetGene(firstGene);
            }
            else
            {
                SetGene(firstGene);
            }

            if (isGeneSet(secondGene))
            {
                UnSetGene(secondGene);
            }
            else
            {
                SetGene(secondGene);
            }
        }

        public override string ToString()
        {

            return Convert.ToString((long)genes, 2);
        }

		public Chromosome Clone()
        {
            return new Chromosome(genes);
        }
    }

    public enum GENE : ulong
    {
        GENE_00 = 0x0000000000000000, //
        GENE_01 = 0x0000000000000001, //
        GENE_02 = 0x0000000000000002, //
        GENE_03 = 0x0000000000000004, //
        GENE_04 = 0x0000000000000008, //
        GENE_05 = 0x0000000000000010, //
        GENE_06 = 0x0000000000000020, //
        GENE_07 = 0x0000000000000040, //
        GENE_08 = 0x0000000000000080, //
        GENE_09 = 0x0000000000000100, //
        GENE_10 = 0x0000000000000200, //
        GENE_11 = 0x0000000000000400, //
        GENE_12 = 0x0000000000000800, //
        GENE_13 = 0x0000000000001000, //
        GENE_14 = 0x0000000000002000, //
        GENE_15 = 0x0000000000004000, //
        GENE_16 = 0x0000000000008000, //
        GENE_17 = 0x0000000000010000, //
        GENE_18 = 0x0000000000020000, //
        GENE_19 = 0x0000000000040000, //
        GENE_20 = 0x0000000000080000, //
        GENE_21 = 0x0000000000100000, //
        GENE_22 = 0x0000000000200000, //
        GENE_23 = 0x0000000000400000, //
        GENE_24 = 0x0000000000800000, //
        GENE_25 = 0x0000000001000000, //
        GENE_26 = 0x0000000002000000, //
        GENE_27 = 0x0000000004000000, //
        GENE_28 = 0x0000000008000000, //
        GENE_29 = 0x0000000010000000, //
        GENE_30 = 0x0000000020000000, //
        GENE_31 = 0x0000000040000000, //
        GENE_32 = 0x0000000080000000, //
        GENE_33 = 0x0000000100000000, //
        GENE_34 = 0x0000000200000000, //
        GENE_35 = 0x0000000400000000, //
        GENE_36 = 0x0000000800000000, //
        GENE_37 = 0x0000001000000000, //
        GENE_38 = 0x0000002000000000, //
        GENE_39 = 0x0000004000000000, //
        GENE_40 = 0x0000008000000000, //
        GENE_41 = 0x0000010000000000, //
        GENE_42 = 0x0000020000000000, //
        GENE_43 = 0x0000040000000000, //
        GENE_44 = 0x0000080000000000, //
        GENE_45 = 0x0000100000000000, //
        GENE_46 = 0x0000200000000000, //
        GENE_47 = 0x0000400000000000, //
        GENE_48 = 0x0000800000000000, //
        GENE_49 = 0x0001000000000000, //
        GENE_50 = 0x0002000000000000, //
        GENE_51 = 0x0004000000000000, //
        GENE_52 = 0x0008000000000000, //
        GENE_53 = 0x0010000000000000, //
        GENE_54 = 0x0020000000000000, //
        GENE_55 = 0x0040000000000000, //
        GENE_56 = 0x0080000000000000, //
        GENE_57 = 0x0100000000000000, //
        GENE_58 = 0x0200000000000000, //
        GENE_59 = 0x0400000000000000, //
        GENE_60 = 0x0800000000000000, //
        GENE_61 = 0x1000000000000000, //
        GENE_62 = 0x2000000000000000, //
        GENE_63 = 0x4000000000000000, //
        GENE_64 = 0x8000000000000000, //
    }
}
*/