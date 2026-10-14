using System.Globalization;
using Day_01_G03;

// Same number format on every machine
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

// Customers.xml and dictionary_english.txt are copied next to the .dll (see the .csproj),
// so we start from that folder. ListGenerator reads Customers.xml using a relative path.
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

Assignment01.Run();
Assignment02.Run();

Console.WriteLine("\nDone.");

// =====================================================================
//  Shared helpers: console output + custom comparers
// =====================================================================

internal static class Display
{
    public static void Title(string text)
    {
        Console.WriteLine($"\n--- {text} ---");
    }

    // Prints the results. Long results show only the first "max" items plus the total count.
    public static void Show<T>(IEnumerable<T> items, int max = 10)
    {
        List<T> list = items.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("  (no results)");
            return;
        }

        foreach (T item in list.Take(max))
        {
            Console.WriteLine($"  {item}");
        }

        if (list.Count > max)
        {
            Console.WriteLine($"  ... and {list.Count - max} more ({list.Count} in total)");
        }
    }
}

// Ordering Q2: custom comparer that compares two strings ignoring the case
internal class CaseInsensitiveStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }
}

// Grouping Q3: two words are "equal" when they consist of the same characters (anagrams)
internal class AnagramComparer : IEqualityComparer<string>
{
    // "form" and "from" both become "fmor"; the Trim() ignores the leading space of " last"
    private static string Normalize(string word)
    {
        return new string(word.Trim().ToCharArray().OrderBy(c => c).ToArray());
    }

    public bool Equals(string x, string y)
    {
        return Normalize(x) == Normalize(y);
    }

    public int GetHashCode(string obj)
    {
        return Normalize(obj).GetHashCode();
    }
}

// =====================================================================
//  Assignment 01 - LINQ
//  Restriction, Element, Aggregate, Ordering and Transformation operators
// =====================================================================

internal static class Assignment01
{
    public static void Run()
    {
        Console.WriteLine("########################################");
        Console.WriteLine("#        ASSIGNMENT 01 - LINQ          #");
        Console.WriteLine("########################################");

        RestrictionOperators();
        ElementOperators();
        AggregateOperators();
        OrderingOperators();
        TransformationOperators();
    }

    // ---------------------------------------------------------------
    //  LINQ - Restriction Operators
    // ---------------------------------------------------------------
    private static void RestrictionOperators()
    {
        Console.WriteLine("\n===== LINQ - Restriction Operators =====");
        List<Product> products = ListGenerator.ProductsList;

        // 1. Find all products that are out of stock.
        Display.Title("1. Products that are out of stock");
        var outOfStock = products.Where(p => p.UnitsInStock == 0);
        Display.Show(outOfStock);

        // 2. Find all products that are in stock and cost more than 3.00 per unit.
        Display.Title("2. Products in stock and UnitPrice > 3.00");
        var inStockAndExpensive = products.Where(p => p.UnitsInStock > 0 && p.UnitPrice > 3.00M);
        Display.Show(inStockAndExpensive);

        // 3. Returns digits whose name is shorter than their value.
        //    (the index of each name in the array IS its digit value, so we use the indexed Where)
        Display.Title("3. Digits whose name is shorter than their value");
        string[] arr = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var shortNames = arr.Where((name, index) => name.Length < index);
        Display.Show(shortNames);
    }

    // ---------------------------------------------------------------
    //  LINQ - Element Operators
    // ---------------------------------------------------------------
    private static void ElementOperators()
    {
        Console.WriteLine("\n===== LINQ - Element Operators =====");
        List<Product> products = ListGenerator.ProductsList;

        // 1. Get first Product out of Stock
        Display.Title("1. First product out of stock");
        Product firstOutOfStock = products.First(p => p.UnitsInStock == 0);
        Console.WriteLine($"  {firstOutOfStock}");

        // 2. Return the first product whose Price > 1000, unless there is no match,
        //    in which case null is returned (that is exactly what FirstOrDefault does).
        Display.Title("2. First product with price > 1000 (or null)");
        Product expensiveProduct = products.FirstOrDefault(p => p.UnitPrice > 1000);
        string expensiveText = expensiveProduct == null ? "null (no product costs more than 1000)" : expensiveProduct.ToString();
        Console.WriteLine($"  {expensiveText}");

        // 3. Retrieve the second number greater than 5
        Display.Title("3. Second number greater than 5");
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
        int secondGreaterThanFive = numbers.Where(n => n > 5).ElementAt(1);
        Console.WriteLine($"  {secondGreaterThanFive}");
    }

    // ---------------------------------------------------------------
    //  LINQ - Aggregate Operators
    // ---------------------------------------------------------------
    private static void AggregateOperators()
    {
        Console.WriteLine("\n===== LINQ - Aggregate Operators =====");
        List<Product> products = ListGenerator.ProductsList;
        List<Customer> customers = ListGenerator.CustomersList;
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

        // 1. Uses Count to get the number of odd numbers in the array
        Display.Title("1. Number of odd numbers in the array");
        int oddCount = numbers.Count(n => n % 2 != 0);
        Console.WriteLine($"  {oddCount}");

        // 2. Return a list of customers and how many orders each has.
        Display.Title("2. Customers and how many orders each has");
        var customerOrders = from c in customers
                             select new { c.CustomerName, OrderCount = c.Orders.Length };
        Display.Show(customerOrders);

        // 3. Return a list of categories and how many products each has
        Display.Title("3. Categories and how many products each has");
        var categoryCounts = from p in products
                             group p by p.Category into g
                             select new { Category = g.Key, ProductCount = g.Count() };
        Display.Show(categoryCounts);

        // 4. Get the total of the numbers in an array.
        Display.Title("4. Total of the numbers in the array");
        int total = numbers.Sum();
        Console.WriteLine($"  {total}");

        // Read dictionary_english.txt into an array of strings first
        string[] words = File.ReadAllLines("dictionary_english.txt");
        Console.WriteLine($"\n  (dictionary_english.txt loaded: {words.Length} words)");

        // 5. Get the total number of characters of all words in dictionary_english.txt
        Display.Title("5. Total number of characters of all words");
        int totalCharacters = words.Sum(w => w.Length);
        Console.WriteLine($"  {totalCharacters}");

        // 6. Get the length of the shortest word in dictionary_english.txt
        Display.Title("6. Length of the shortest word");
        int shortestLength = words.Min(w => w.Length);
        Console.WriteLine($"  {shortestLength}");

        // 7. Get the length of the longest word in dictionary_english.txt
        Display.Title("7. Length of the longest word");
        int longestLength = words.Max(w => w.Length);
        Console.WriteLine($"  {longestLength}");

        // 8. Get the average length of the words in dictionary_english.txt
        Display.Title("8. Average length of the words");
        double averageLength = words.Average(w => w.Length);
        Console.WriteLine($"  {averageLength:F2}");
    }

    // ---------------------------------------------------------------
    //  LINQ - Ordering Operators
    // ---------------------------------------------------------------
    private static void OrderingOperators()
    {
        Console.WriteLine("\n===== LINQ - Ordering Operators =====");
        List<Product> products = ListGenerator.ProductsList;
        string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        string[] fruits = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

        // 1. Sort a list of products by name
        Display.Title("1. Products sorted by name");
        var byName = products.OrderBy(p => p.ProductName);
        Display.Show(byName);

        // 2. Uses a custom comparer to do a case-insensitive sort of the words in an array.
        Display.Title("2. Words sorted with a custom case-insensitive comparer");
        var caseInsensitive = fruits.OrderBy(w => w, new CaseInsensitiveStringComparer());
        Display.Show(caseInsensitive);

        // 3. Sort a list of products by units in stock from highest to lowest.
        Display.Title("3. Products sorted by units in stock (highest to lowest)");
        var byStockDescending = products.OrderByDescending(p => p.UnitsInStock);
        Display.Show(byStockDescending);

        // 4. Sort a list of digits, first by length of their name, and then alphabetically by the name itself.
        Display.Title("4. Digits sorted by name length, then alphabetically");
        var digitsSorted = digits.OrderBy(d => d.Length).ThenBy(d => d);
        Display.Show(digitsSorted);

        // 5. Sort first by word length and then by a case-insensitive sort of the words in an array.
        Display.Title("5. Words sorted by length, then case-insensitive");
        var lengthThenCaseInsensitive = fruits.OrderBy(w => w.Length)
                                              .ThenBy(w => w, new CaseInsensitiveStringComparer());
        Display.Show(lengthThenCaseInsensitive);

        // 6. Sort a list of products, first by category, and then by unit price, from highest to lowest.
        Display.Title("6. Products sorted by category, then by unit price (highest to lowest)");
        var byCategoryThenPrice = products.OrderBy(p => p.Category)
                                          .ThenByDescending(p => p.UnitPrice);
        Display.Show(byCategoryThenPrice);

        // 7. Sort first by word length and then by a case-insensitive descending sort of the words.
        Display.Title("7. Words sorted by length, then case-insensitive descending");
        var lengthThenDescending = fruits.OrderBy(w => w.Length)
                                         .ThenByDescending(w => w, new CaseInsensitiveStringComparer());
        Display.Show(lengthThenDescending);

        // 8. Create a list of all digits in the array whose second letter is 'i'
        //    that is reversed from the order in the original array.
        Display.Title("8. Digits whose second letter is 'i' (reversed order)");
        var secondLetterI = digits.Where(d => d[1] == 'i').Reverse();
        Display.Show(secondLetterI);
    }

    // ---------------------------------------------------------------
    //  LINQ - Transformation Operators
    // ---------------------------------------------------------------
    private static void TransformationOperators()
    {
        Console.WriteLine("\n===== LINQ - Transformation Operators =====");
        List<Product> products = ListGenerator.ProductsList;
        List<Customer> customers = ListGenerator.CustomersList;

        // 1. Return a sequence of just the names of a list of products.
        Display.Title("1. Names of the products");
        var productNames = products.Select(p => p.ProductName);
        Display.Show(productNames);

        // 2. Produce a sequence of the uppercase and lowercase versions of each word (Anonymous Types).
        Display.Title("2. Uppercase and lowercase version of each word");
        string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };
        var upperAndLower = words.Select(w => new { Upper = w.ToUpper(), Lower = w.ToLower() });
        Display.Show(upperAndLower);

        // 3. Produce a sequence containing some properties of Products,
        //    including UnitPrice which is renamed to Price in the resulting type.
        Display.Title("3. Some properties of the products (UnitPrice renamed to Price)");
        var productInfo = products.Select(p => new { p.ProductName, p.Category, Price = p.UnitPrice });
        Display.Show(productInfo);

        // 4. Determine if the value of int in an array match their position in the array.
        Display.Title("4. Does the value match its position?");
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
        var inPlace = numbers.Select((n, index) => new { Number = n, InPlace = n == index });
        Console.WriteLine("  Number: In-place?");
        foreach (var item in inPlace)
        {
            Console.WriteLine($"  {item.Number}: {item.InPlace}");
        }

        // 5. Returns all pairs of numbers from both arrays such that the number from numbersA
        //    is less than the number from numbersB.
        Display.Title("5. Pairs where a < b");
        int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
        int[] numbersB = { 1, 3, 5, 7, 8 };
        var pairs = from a in numbersA
                    from b in numbersB
                    where a < b
                    select new { a, b };
        Console.WriteLine("  Pairs where a < b:");
        foreach (var pair in pairs)
        {
            Console.WriteLine($"  {pair.a} is less than {pair.b}");
        }

        // 6. Select all orders where the order total is less than 500.00.
        Display.Title("6. Orders with a total less than 500.00");
        var smallOrders = from c in customers
                          from o in c.Orders
                          where o.Total < 500.00M
                          select o;
        Display.Show(smallOrders);

        // 7. Select all orders where the order was made in 1998 or later.
        Display.Title("7. Orders made in 1998 or later");
        var recentOrders = from c in customers
                           from o in c.Orders
                           where o.OrderDate.Year >= 1998
                           select o;
        Display.Show(recentOrders);
    }
}

// =====================================================================
//  Assignment 02 - LINQ
//  Element, Aggregate, Set, Partitioning, Quantifiers and Grouping operators
// =====================================================================

internal static class Assignment02
{
    public static void Run()
    {
        Console.WriteLine("\n\n########################################");
        Console.WriteLine("#        ASSIGNMENT 02 - LINQ          #");
        Console.WriteLine("########################################");

        ElementOperators();
        AggregateOperators();
        SetOperators();
        PartitioningOperators();
        Quantifiers();
        GroupingOperators();
    }

    // ---------------------------------------------------------------
    //  LINQ - Element Operators
    // ---------------------------------------------------------------
    private static void ElementOperators()
    {
        Console.WriteLine("\n===== LINQ - Element Operators =====");
        List<Product> products = ListGenerator.ProductsList;

        // 1. Get first Product out of Stock
        Display.Title("1. First product out of stock");
        Product firstOutOfStock = products.First(p => p.UnitsInStock == 0);
        Console.WriteLine($"  {firstOutOfStock}");

        // 2. Return the first product whose Price > 1000, unless there is no match,
        //    in which case null is returned (that is exactly what FirstOrDefault does).
        Display.Title("2. First product with price > 1000 (or null)");
        Product expensiveProduct = products.FirstOrDefault(p => p.UnitPrice > 1000);
        string expensiveText = expensiveProduct == null ? "null (no product costs more than 1000)" : expensiveProduct.ToString();
        Console.WriteLine($"  {expensiveText}");

        // 3. Retrieve the second number greater than 5
        Display.Title("3. Second number greater than 5");
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
        int secondGreaterThanFive = numbers.Where(n => n > 5).ElementAt(1);
        Console.WriteLine($"  {secondGreaterThanFive}");
    }

    // ---------------------------------------------------------------
    //  LINQ - Aggregate Operators
    // ---------------------------------------------------------------
    private static void AggregateOperators()
    {
        Console.WriteLine("\n===== LINQ - Aggregate Operators =====");
        List<Product> products = ListGenerator.ProductsList;
        List<Customer> customers = ListGenerator.CustomersList;
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

        // 1. Uses Count to get the number of odd numbers in the array
        Display.Title("1. Number of odd numbers in the array");
        int oddCount = numbers.Count(n => n % 2 != 0);
        Console.WriteLine($"  {oddCount}");

        // 2. Return a list of customers and how many orders each has.
        Display.Title("2. Customers and how many orders each has");
        var customerOrders = from c in customers
                             select new { c.CustomerName, OrderCount = c.Orders.Length };
        Display.Show(customerOrders);

        // 3. Return a list of categories and how many products each has
        Display.Title("3. Categories and how many products each has");
        var categoryCounts = from p in products
                             group p by p.Category into g
                             select new { Category = g.Key, ProductCount = g.Count() };
        Display.Show(categoryCounts);

        // 4. Get the total of the numbers in an array.
        Display.Title("4. Total of the numbers in the array");
        int total = numbers.Sum();
        Console.WriteLine($"  {total}");

        // Read dictionary_english.txt into an array of strings first
        string[] words = File.ReadAllLines("dictionary_english.txt");
        Console.WriteLine($"\n  (dictionary_english.txt loaded: {words.Length} words)");

        // 5. Get the total number of characters of all words in dictionary_english.txt
        Display.Title("5. Total number of characters of all words");
        int totalCharacters = words.Sum(w => w.Length);
        Console.WriteLine($"  {totalCharacters}");

        // 6. Get the length of the shortest word in dictionary_english.txt
        Display.Title("6. Length of the shortest word");
        int shortestLength = words.Min(w => w.Length);
        Console.WriteLine($"  {shortestLength}");

        // 7. Get the length of the longest word in dictionary_english.txt
        Display.Title("7. Length of the longest word");
        int longestLength = words.Max(w => w.Length);
        Console.WriteLine($"  {longestLength}");

        // 8. Get the average length of the words in dictionary_english.txt
        Display.Title("8. Average length of the words");
        double averageLength = words.Average(w => w.Length);
        Console.WriteLine($"  {averageLength:F2}");

        // 9. Get the total units in stock for each product category.
        Display.Title("9. Total units in stock for each category");
        var unitsPerCategory = from p in products
                               group p by p.Category into g
                               select new { Category = g.Key, TotalUnits = g.Sum(p => p.UnitsInStock) };
        foreach (var item in unitsPerCategory)
        {
            Console.WriteLine($"  {item.Category}: {item.TotalUnits}");
        }

        // 10. Get the cheapest price among each category's products
        Display.Title("10. Cheapest price in each category");
        var cheapestPrice = from p in products
                            group p by p.Category into g
                            select new { Category = g.Key, CheapestPrice = g.Min(p => p.UnitPrice) };
        foreach (var item in cheapestPrice)
        {
            Console.WriteLine($"  {item.Category}: {item.CheapestPrice:F2}");
        }

        // 11. Get the products with the cheapest price in each category (Use Let)
        Display.Title("11. Products with the cheapest price in each category (let)");
        var cheapestProducts = from p in products
                               group p by p.Category into g
                               let minPrice = g.Min(p => p.UnitPrice)
                               select new { Category = g.Key, Cheapest = g.Where(p => p.UnitPrice == minPrice) };
        foreach (var item in cheapestProducts)
        {
            Console.WriteLine($"  {item.Category}:");
            foreach (var product in item.Cheapest)
            {
                Console.WriteLine($"      {product.ProductName} ({product.UnitPrice:F2})");
            }
        }

        // 12. Get the most expensive price among each category's products.
        Display.Title("12. Most expensive price in each category");
        var mostExpensivePrice = from p in products
                                 group p by p.Category into g
                                 select new { Category = g.Key, MaxPrice = g.Max(p => p.UnitPrice) };
        foreach (var item in mostExpensivePrice)
        {
            Console.WriteLine($"  {item.Category}: {item.MaxPrice:F2}");
        }

        // 13. Get the products with the most expensive price in each category.
        Display.Title("13. Products with the most expensive price in each category");
        var mostExpensiveProducts = from p in products
                                    group p by p.Category into g
                                    let maxPrice = g.Max(p => p.UnitPrice)
                                    select new { Category = g.Key, MostExpensive = g.Where(p => p.UnitPrice == maxPrice) };
        foreach (var item in mostExpensiveProducts)
        {
            Console.WriteLine($"  {item.Category}:");
            foreach (var product in item.MostExpensive)
            {
                Console.WriteLine($"      {product.ProductName} ({product.UnitPrice:F2})");
            }
        }

        // 14. Get the average price of each category's products.
        Display.Title("14. Average price of each category's products");
        var averagePrice = from p in products
                           group p by p.Category into g
                           select new { Category = g.Key, AveragePrice = g.Average(p => p.UnitPrice) };
        foreach (var item in averagePrice)
        {
            Console.WriteLine($"  {item.Category}: {item.AveragePrice:F2}");
        }
    }

    // ---------------------------------------------------------------
    //  LINQ - Set Operators
    // ---------------------------------------------------------------
    private static void SetOperators()
    {
        Console.WriteLine("\n===== LINQ - Set Operators =====");
        List<Product> products = ListGenerator.ProductsList;
        List<Customer> customers = ListGenerator.CustomersList;

        // First letter of every product name and of every customer name
        var productLetters = products.Select(p => p.ProductName[0]);
        var customerLetters = customers.Select(c => c.CustomerName[0]);

        // 1. Find the unique Category names from Product List
        Display.Title("1. Unique category names");
        var uniqueCategories = products.Select(p => p.Category).Distinct();
        Display.Show(uniqueCategories);

        // 2. Produce a Sequence containing the unique first letter from both product and customer names.
        Display.Title("2. Unique first letters from both product and customer names (Union)");
        var allLetters = productLetters.Union(customerLetters).OrderBy(ch => ch);
        Console.WriteLine($"  {string.Join(", ", allLetters)}");

        // 3. Create one sequence that contains the common first letter from both product and customer names.
        Display.Title("3. Common first letters (Intersect)");
        var commonLetters = productLetters.Intersect(customerLetters).OrderBy(ch => ch);
        Console.WriteLine($"  {string.Join(", ", commonLetters)}");

        // 4. Create one sequence that contains the first letters of product names
        //    that are not also first letters of customer names.
        Display.Title("4. First letters of products that are not first letters of customers (Except)");
        var onlyProductLetters = productLetters.Except(customerLetters).OrderBy(ch => ch);
        Console.WriteLine($"  {string.Join(", ", onlyProductLetters)}");

        // 5. Create one sequence that contains the last three characters in each name of all
        //    customers and products, including any duplicates (Concat keeps the duplicates).
        Display.Title("5. Last three characters of every customer and product name (Concat)");
        var allNames = customers.Select(c => c.CustomerName).Concat(products.Select(p => p.ProductName));
        var lastThree = allNames.Select(n => n.Length >= 3 ? n.Substring(n.Length - 3) : n);
        Display.Show(lastThree);
    }

    // ---------------------------------------------------------------
    //  LINQ - Partitioning Operators
    // ---------------------------------------------------------------
    private static void PartitioningOperators()
    {
        Console.WriteLine("\n===== LINQ - Partitioning Operators =====");
        List<Customer> customers = ListGenerator.CustomersList;
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

        // All the orders of the customers in Washington (Region = "WA")
        var washingtonOrders = from c in customers
                               where c.Region == "WA"
                               from o in c.Orders
                               select o;

        // 1. Get the first 3 orders from customers in Washington
        Display.Title("1. First 3 orders from customers in Washington");
        Display.Show(washingtonOrders.Take(3));

        // 2. Get all but the first 2 orders from customers in Washington.
        Display.Title("2. All but the first 2 orders from customers in Washington");
        Display.Show(washingtonOrders.Skip(2));

        // 3. Return elements starting from the beginning of the array until a number
        //    is hit that is less than its position in the array.
        Display.Title("3. TakeWhile: until a number is less than its position");
        var untilLessThanPosition = numbers.TakeWhile((n, index) => n >= index);
        Console.WriteLine($"  {string.Join(", ", untilLessThanPosition)}");

        // 4. Get the elements of the array starting from the first element divisible by 3.
        Display.Title("4. SkipWhile: starting from the first element divisible by 3");
        var fromFirstDivisibleByThree = numbers.SkipWhile(n => n % 3 != 0);
        Console.WriteLine($"  {string.Join(", ", fromFirstDivisibleByThree)}");

        // 5. Get the elements of the array starting from the first element less than its position.
        Display.Title("5. SkipWhile: starting from the first element less than its position");
        var fromFirstLessThanPosition = numbers.SkipWhile((n, index) => n >= index);
        Console.WriteLine($"  {string.Join(", ", fromFirstLessThanPosition)}");
    }

    // ---------------------------------------------------------------
    //  LINQ - Quantifiers
    // ---------------------------------------------------------------
    private static void Quantifiers()
    {
        Console.WriteLine("\n===== LINQ - Quantifiers =====");
        List<Product> products = ListGenerator.ProductsList;

        // 1. Determine if any of the words in dictionary_english.txt contain the substring 'ei'.
        Display.Title("1. Does any word contain the substring 'ei'?");
        string[] words = File.ReadAllLines("dictionary_english.txt");
        bool anyWordHasEi = words.Any(w => w.Contains("ei"));
        Console.WriteLine($"  {anyWordHasEi}");

        // 2. Return a grouped list of products only for categories
        //    that have at least one product that is out of stock.
        Display.Title("2. Categories that have at least one product out of stock");
        var categoriesWithOutOfStock = from p in products
                                       group p by p.Category into g
                                       where g.Any(p => p.UnitsInStock == 0)
                                       select g;
        foreach (var grp in categoriesWithOutOfStock)
        {
            Console.WriteLine($"  {grp.Key}");
            foreach (var product in grp)
            {
                Console.WriteLine($"      {product.ProductName} (in stock: {product.UnitsInStock})");
            }
        }

        // 3. Return a grouped list of products only for categories
        //    that have all of their products in stock.
        Display.Title("3. Categories that have all of their products in stock");
        var categoriesAllInStock = from p in products
                                   group p by p.Category into g
                                   where g.All(p => p.UnitsInStock > 0)
                                   select g;
        foreach (var grp in categoriesAllInStock)
        {
            Console.WriteLine($"  {grp.Key}");
            foreach (var product in grp)
            {
                Console.WriteLine($"      {product.ProductName} (in stock: {product.UnitsInStock})");
            }
        }
    }

    // ---------------------------------------------------------------
    //  LINQ - Grouping Operators
    // ---------------------------------------------------------------
    private static void GroupingOperators()
    {
        Console.WriteLine("\n===== LINQ - Grouping Operators =====");

        // 1. Use group by to partition a list of numbers by their remainder when divided by 5
        Display.Title("1. Numbers grouped by their remainder when divided by 5");
        List<int> numbers = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        var byRemainder = from n in numbers
                          group n by n % 5 into g
                          select g;
        foreach (var grp in byRemainder)
        {
            Console.WriteLine($"Numbers with a remainder of {grp.Key} when divided by 5:");
            foreach (var number in grp)
            {
                Console.WriteLine(number);
            }
        }

        // 2. Use group by to partition a list of words by their first letter (dictionary_english.txt).
        //    (there are 370,000 words, so we print the number of words of every letter and 3 examples)
        Display.Title("2. Words grouped by their first letter");
        string[] words = File.ReadAllLines("dictionary_english.txt");
        var byFirstLetter = from w in words
                            where w.Length > 0
                            group w by w[0] into g
                            orderby g.Key
                            select g;
        foreach (var grp in byFirstLetter)
        {
            Console.WriteLine($"  '{grp.Key}': {grp.Count()} words, e.g. {string.Join(", ", grp.Take(3))}");
        }

        // 3. Group By with a custom comparer that matches the words that consist of the same characters
        Display.Title("3. Words that consist of the same characters, grouped together (custom comparer)");
        string[] arr = { "from", "salt", "earn", " last", "near", "form" };
        var anagramGroups = arr.GroupBy(w => w, new AnagramComparer());
        foreach (var grp in anagramGroups)
        {
            foreach (var word in grp)
            {
                Console.WriteLine(word.Trim());
            }
            Console.WriteLine("....");
        }
    }
}
