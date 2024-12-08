namespace Foodiefeed_api.entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }      
        
    }

    public class Tags
    {
        public static readonly List<string> names = new List<string>
        {
            // Vegies
            "spaghetti", "tomato", "vegetables", "chicken", "beef", "pasta", "onion", "garlic",
            "carrot", "potato", "broccoli", "spinach", "kale", "bell pepper", "cabbage",
            "zucchini", "cauliflower", "mushrooms", "asparagus", "corn", "peas", "eggplant",
            "pumpkin", "sweet potato", "celery", "lettuce", "cucumber", "radish", "beetroot",
            "artichoke", "green beans", "brussels sprouts", "arugula", "fennel", "chard",
            "leek", "shallot", "ginger", "jalapeño", "chili pepper", "scallion", "bok choy",
            "squash", "collard greens", "swiss chard", "pumpkin puree", "black beans",
            "turnip", "parsnip", "kohlrabi", "jicama", "butternut squash", "sweet corn",
            "snow peas", "sugar snap peas", "baby corn", "endive", "radicchio", "savoy cabbage",
            "watercress", "dulse", "seaweed", "nori", "pumpkin seeds", "cabbage sprouts", 

            // Fruits
            "coconut", "banana", "apple", "orange", "grapefruit", "lemon", "lime", "berries",
            "strawberries", "blueberries", "raspberries", "blackberries", "kiwi", "pear",
            "pineapple", "peach", "plum", "cherry", "watermelon", "cantaloupe", "honeydew",
            "grapes", "pomegranate", "fig", "date", "mango", "passionfruit", "clementine",
            "tangerine", "apricot", "nectarine", "dragonfruit", "guava", "jackfruit",
            "starfruit", "papaya", "persimmon", "lychee", "rhubarb", "blood orange",
            "coconut water", 

            // Meat
            "chicken breast", "chicken thighs", "ground beef", "steak", "pork", "bacon",
            "ham", "lamb", "turkey", "duck", "fish", "salmon", "tuna", "shrimp", "crab",
            "lobster", "scallops", "clams", "mussels", "octopus", "sardines", "anchovies",
            "chorizo", "salami", "hot dog", "venison", "bison", "quail", 

            //Dairy
            "milk", "cream", "heavy cream", "yogurt", "sour cream", "buttermilk",
            "cheese", "mozzarella", "cheddar", "parmesan", "feta", "goat cheese",
            "ricotta", "cream cheese", "cottage cheese", "ice cream", "whipped cream",
            "kefir", "evaporated milk", "sweetened condensed milk", "powdered milk",
            "non-dairy creamer", "coconut yogurt", "almond milk", "soy yogurt", 

            // Cereals and starch
            "bread", "rice", "quinoa", "barley", "oats", "bulgur", "millet", "couscous",
            "pasta", "flour", "cornmeal", "polenta", "tortillas", "crackers", "muesli",
            "granola", "whole wheat bread", "rye bread", "pita bread", "naan",
            "sourdough", "baguette", "pasta shells", "elbow macaroni", 

            // Nuts and seeds
            "walnuts", "almonds", "pecans", "cashews", "hazelnuts", "macadamia nuts",
            "pistachios", "pumpkin seeds", "sunflower seeds", "chia seeds", "flaxseeds",
            "sesame seeds", "hemp seeds", "pine nuts", "walnut oil", "peanut butter",
            "almond butter", "sunflower butter", 

            // Spices and herbs
            "salt", "pepper", "sugar", "brown sugar", "honey", "maple syrup", "basil",
            "oregano", "thyme", "rosemary", "parsley", "cilantro", "dill", "tarragon",
            "sage", "chili flakes", "cinnamon", "nutmeg", "paprika", "cumin", "coriander",
            "cardamom", "ginger", "vanilla", "vanilla extract", "cocoa powder",
            "baking powder", "baking soda", "yeast", "olive oil", "vegetable oil",
            "coconut oil", "butter", "margarine", "sesame oil", "peanut oil", "hot sauce",
            "fish sauce", "soy sauce", "worcestershire sauce", "sriracha", 

            //Add-ons
            "salsa", "syrup", "vinegar", "mustard", "ketchup", "mayonnaise", "barbecue sauce",
            "teriyaki sauce", "pesto", "tahini", "hummus", "peanut sauce", "soy yogurt", 

            //Other
            "tofu", "tempeh", "seitan", "nut butter", "coconut milk", "almond milk",
            "soy milk", "rice milk", "nutritional yeast", "protein powder",
            "meal replacement", "frozen vegetables", "frozen fruits",
            "canned beans", "canned tomatoes", "canned corn", "canned tuna",
            "broth", "stock", "gelatin", "instant noodles",
            "pasta sauce", "pizza sauce", "curry paste", "chili paste", "salsa verde",
    
            //Meals
            "chicken curry", "beef stew", "vegetable stir fry", "spaghetti carbonara",
            "tuna salad", "lasagna", "chicken alfredo", "chili con carne", "ratatouille",
            "pad thai", "fish tacos", "sushi rolls", "stuffed peppers", "vegetable soup",
            "egg drop soup", "tom yum soup", "clams chowder", "french onion soup",
            "chocolate cake", "cheesecake", "pancakes", "waffles", "muffins",
            "scones", "cookies", "brownies", "banana bread", "crêpes",
            "quiche", "frittata", "omelette", "gratin", "pot pie", "spring rolls",
            "dumplings", "casserole", "shakshuka", "foie gras", "tiramisu"
        };
    }
}
