/*
=====================================================================================================================

GA Algorithm to find the shortest path between 2 input nodes of a complete graph

Genetic Algorithm (GA) Explanation:
The Genetic Algorithm is inspired by the process of natural selection. It starts with a population of candidate solutions (individuals), 
and through the processes of selection, crossover, and mutation, it iteratively evolves the population to optimize the solution.

Each candidate solution is a potential path from the start node to the end node, and the goal is to minimize the total distance of the path. 
The "fitness" of a candidate is inversely proportional to its path length — shorter paths have higher fitness.

Key Components of GA in this Code:
- **Population**: A collection of paths (individuals) in the current generation. Each path represents a potential solution.
- **Fitness**: A measure of the quality of a path (lower distance means higher fitness).
- **Selection**: The process of choosing paths for reproduction based on fitness.
- **Crossover**: The process of combining two parent paths to create offspring, aiming to inherit the best traits from both parents.
- **Mutation**: The process of introducing random changes in a path to maintain diversity and explore new solutions.

Steps in the GA Algorithm:
1. **Initialization**:
   - Generate an initial population of random paths. Each path starts with the startNode, ends with the endNode, and includes intermediate cities in random order.

2. **Fitness Evaluation**:
   - For each individual in the population, calculate its fitness based on the total distance of the path. Shorter paths receive higher fitness.

3. **Selection**:
   - Select the top-performing individuals based on fitness. These will form the parent pool for reproduction.

4. **Crossover**:
   - Randomly pair parents and create offspring using ordered crossover. The offspring inherit a portion of the path from one parent and the remaining cities from the other parent.

5. **Mutation**:
   - With a small probability, randomly swap two cities in the path to introduce variation and prevent premature convergence.

6. **Next Generation**:
   - Combine the parents and offspring to form the next generation. This ensures the population size remains constant.

7. **Repeat**:
   - Repeat the process of fitness evaluation, selection, crossover, and mutation for a fixed number of generations. Over time, the population evolves, and the best paths converge toward the optimal solution.

8. **Result**:
   - After the specified number of generations, the algorithm outputs the best path found and its total length.

Key Advantages of GA:
- **Exploration**: The random initialization and mutation allow the algorithm to explore a wide range of solutions.
- **Exploitation**: The selection and crossover focus on refining the best solutions.

Key Parameters in GA:
- **Population Size**: Determines how many paths are maintained in each generation.
- **Generations**: Controls how many iterations the algorithm will perform.
- **Mutation Rate**: Balances exploration and exploitation by controlling the frequency of random changes in paths.

=====================================================================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;

public class ShortestPathGA
{
    private int cityCount; // Number of cities
    private double[,] distanceMatrix; // Matrix holding distances between cities
    private Random rand = new Random(); // Random object for generating random numbers

    // Genetic Algorithm parameters
    private int populationSize = 100; // Number of individuals in the population
    private int generations = 500; // Number of generations to evolve
    private double mutationRate = 0.1; // Probability of mutation

    public ShortestPathGA(int cityCount, double[,] distanceMatrix)
    {
        this.cityCount = cityCount;
        this.distanceMatrix = distanceMatrix;
    }

    // Method to find the shortest path using Genetic Algorithm
    public void FindShortestPath(int startNode, int endNode)
    {
        // Initialize population
        List<List<int>> population = InitializePopulation(startNode, endNode);

        double bestLength = double.MaxValue;
        List<int> bestPath = new List<int>();

        // Iterate through generations
        for (int gen = 0; gen < generations; gen++)
        {
            // Evaluate fitness (path length)
            // Create a list to store the evaluated population with paths and their fitness values
            List<(List<int> Path, double Fitness)> evaluatedPopulation = new List<(List<int> Path, double Fitness)>();

            // Iterate over each individual in the population
            foreach (var individual in population)
            {
                // Calculate the fitness (path length) for the current individual
                double fitness = CalculatePathLength(individual);

                // Add the individual and its fitness to the evaluated population
                evaluatedPopulation.Add((individual, fitness));
            }
            // Sort the evaluated population by fitness in ascending order (shorter paths are better)
            evaluatedPopulation = evaluatedPopulation.OrderBy(x => x.Fitness).ToList();

            // Track the best path in the current generation
            if (evaluatedPopulation[0].Fitness < bestLength)
            {
                bestLength = evaluatedPopulation[0].Fitness;
                bestPath = new List<int>(evaluatedPopulation[0].Path);
            }

            // Perform selection
            List<List<int>> selectedPopulation = SelectPopulation(evaluatedPopulation);

            // Perform crossover to produce offspring
            List<List<int>> offspring = CrossoverPopulation(selectedPopulation, startNode, endNode);

            // Apply mutation
            MutatePopulation(offspring);

            // Set the new population for the next generation
            population = offspring;

            Console.WriteLine($"Generation {gen + 1}, Best Length: {bestLength}");
        }

        // Output the best path found
        Console.WriteLine("Best Path: " + string.Join(" -> ", bestPath));
        Console.WriteLine($"Total Length: {bestLength}");
    }

    // Initialize the population with random paths
    private List<List<int>> InitializePopulation(int startNode, int endNode)
    {
        List<List<int>> population = new List<List<int>>();
        // Loop to generate the population
        for (int i = 0; i < populationSize; i++)
        {
            // Create a list of all city indices from 0 to cityCount - 1
            List<int> allCities = new List<int>();
            for (int j = 0; j < cityCount; j++)
            {
                allCities.Add(j);
            }

            // Remove the startNode and endNode from the list of cities
            List<int> intermediateCities = new List<int>();
            foreach (var city in allCities)
            {
                if (city != startNode && city != endNode)
                {
                    intermediateCities.Add(city);
                }
            }

            // Shuffle the intermediate cities randomly
            List<int> shuffledCities = new List<int>();
            while (intermediateCities.Count > 0)
            {
                int randomIndex = rand.Next(intermediateCities.Count);
                shuffledCities.Add(intermediateCities[randomIndex]);
                intermediateCities.RemoveAt(randomIndex);
            }

            // Create the individual path
            List<int> individual = new List<int>();

            // Add the startNode at the beginning of the path
            individual.Add(startNode);

            // Add the shuffled intermediate cities
            individual.AddRange(shuffledCities);

            // Add the endNode at the end of the path
            individual.Add(endNode);

            // Add the individual path to the population
            population.Add(individual);
        }

        // Return the generated population
        return population;
    }

    // Calculate the total length of a given path (for fitness)
    private double CalculatePathLength(List<int> path)
    {
        double length = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            length += distanceMatrix[path[i], path[i + 1]];
        }
        return length;
    }

    // Select the top individuals from the population based on fitness
    private List<List<int>> SelectPopulation(List<(List<int> Path, double Fitness)> evaluatedPopulation)
    {
        // Calculate the number of individuals to select (half the population size)
        int selectionCount = populationSize / 2;

        // Create an empty list to store the selected individuals
        List<List<int>> selectedPopulation = new List<List<int>>();

        // Iterate through the top individuals in the evaluated population
        for (int i = 0; i < selectionCount; i++)
        {
            // Get the current individual (path) from the evaluated population
            List<int> path = new List<int>(evaluatedPopulation[i].Path);

            // Add the path to the selected population
            selectedPopulation.Add(path);
        }

        // Return the selected population
        return selectedPopulation;
    }

    // Perform crossover to produce offspring
    private List<List<int>> CrossoverPopulation(List<List<int>> selectedPopulation, int startNode, int endNode)
    {
        List<List<int>> offspring = new List<List<int>>();

        while (offspring.Count < populationSize)
        {
            // Select two random parents
            List<int> parent1 = selectedPopulation[rand.Next(selectedPopulation.Count)];
            List<int> parent2 = selectedPopulation[rand.Next(selectedPopulation.Count)];

            // Perform ordered crossover
            int crossoverPoint1 = rand.Next(1, parent1.Count - 2);
            int crossoverPoint2 = rand.Next(crossoverPoint1, parent1.Count - 1);

            // Create an empty list to store the child (offspring)
            List<int> child = new List<int>();

            // Add the first part of the path from parent1 (up to crossoverPoint1)
            for (int i = 0; i < crossoverPoint1; i++)
            {
                child.Add(parent1[i]);
            }

            // Add the remaining cities from parent2 that are not already in the child
            foreach (int city in parent2)
            {
                if (!child.Contains(city))
                {
                    child.Add(city);
                }
            }

            // Add the endNode to the child
            child.Add(endNode);

            // Add the completed child to the offspring list
            offspring.Add(child);
        }

        return offspring;
    }

    // Apply mutation to the population
    private void MutatePopulation(List<List<int>> population)
    {
        foreach (var individual in population)
        {
            if (rand.NextDouble() < mutationRate)
            {
                int index1 = rand.Next(1, individual.Count - 2);
                int index2 = rand.Next(1, individual.Count - 2);

                // Swap two cities in the path
                (individual[index1], individual[index2]) = (individual[index2], individual[index1]);
            }
        }
    }

    // Main method to accept inputs and run the algorithm
    public static void Main(string[] args)
    {
        // Example distance matrix
        double[,] distanceMatrix = {
            { 0, 2, 1, 14 },
            { 7, 0, 7, 8 },
            { 5, 1, 0, 2 },
            { 1, 3, 5, 0 }
        };

        //double[,] distanceMatrix = {
        //{ 0, 1, 15, 20, 25, 30, 35, 40, 45, 50 },
        //{ 10, 0, 1, 15, 20, 25, 30, 35, 40, 45 },
        //{ 15, 10, 0, 1, 15, 20, 25, 30, 35, 40 },
        //{ 20, 15, 10, 0, 1, 15, 20, 25, 30, 35 },
        //{ 25, 20, 15, 10, 0, 1, 15, 20, 25, 30 },
        //{ 30, 25, 20, 15, 10, 0, 1, 15, 20, 25 },
        //{ 35, 30, 25, 20, 15, 10, 0, 10, 1, 20 },
        //{ 40, 35, 30, 25, 20, 15, 10, 0, 1, 15 },
        //{ 45, 40, 35, 30, 25, 20, 15, 10, 0, 1 },
        //{ 50, 45, 40, 35, 30, 25, 20, 15, 10, 0 }
        //};

        // Create the ShortestPathGA solver
        ShortestPathGA solver = new ShortestPathGA(4, distanceMatrix);

        // Input the start and end nodes (cities)
        Console.Write("Enter the start city index: ");
        int startNode = int.Parse(Console.ReadLine());

        Console.Write("Enter the end city index: ");
        int endNode = int.Parse(Console.ReadLine());

        // Find the shortest path
        solver.FindShortestPath(startNode, endNode);
    }
}
