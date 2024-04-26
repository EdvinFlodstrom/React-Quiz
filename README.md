Journal
=======
This is a quiz project. One could propably deduce as much from the repository name. So this project will be a lot like one I previously made (it's simply called 'Quiz'). This one will be basically identical, but with some different functions and whatnot. Primarily I want this one to be more smooth, and not as cluttered with old and outdated stuff. And I won't have three separate user interfaces this time. The React app will do for now. I'll be using an SQL database that I'll communicate with using an ASP.NET web API and EF Core.

2024-04-20
-----------
Who needs spare time anyway? I need more REST. So I'll start by preparing the database creation and migrations and such using EF Core.

Several hours later, I've now successfully set up the SQL database. It took me quite a while, with a lot of struggles along the way, but the database has now been created using only EF Core. No SQL queries needed! Seeing that SQL database set up was mighty fine, indeed. I'll continue work on it tomorrow.

2024-04-21
----------
Time to start setting up the controller, the service, and Mediator in the backend. I'll be using all of this to interact with the SQL database manually through Postman. I think I'll create a POST endpoint for creating questions that won't initially be available from the frontend. I'll need some questions for testing the quiz, after all.

I've now been at this for a few hours, and I'll be leaving for a bit. I've successfully added an endpoint to my controller class, and it should be fully funcional. I've been working on the related MediatR command and the quiz class, but the endpoint is not fully ready yet. I also created a 'FloatingIds' table in the SQL database. It always worries me to create a new migration and update the database, and I was real worried for a second when I updated it and didn't see the new table. Took me about five seconds to realize that I'd forgotten to map the new model class to a table in my DbContext class. So I fixed that, created a new migration, and updated the database. And just like that, new table! It's really fun seeing EF Core interact so directly with my SQL database like this. By the way, the 'FloatingIds' table will contain the IDs of questions that have been deleted. Should any IDs exist here at the time of a question being created, that ID will be used. Otherwise, the new question's ID will be the number of questions in the table + 1.

Right, it took me a while to notice that, remotely, the folder 'Backend' within 'Backend' was apparently called 'backend'. So instead of 'Backend/Backend', it was 'Backend/backend'. I'd consider ignoring it if it was only for visual purposes. But it was actually a bit more of a problem than that. Namely, when I'd made changes to 'Backend', I was unable to add them normally. Git considered the folder to be 'backend', so my commands weren't exactly recognized. I was able to get around this by manually typing 'backend', but what kind of programmer doesn't choose the easy way when they can? So I ran `git mv backend tempname` and `git mv tempname Backend`. And just like that, the folder's now called 'Backend'. Remotely, and locally. Ah, and this caused me to notice that, alas, I've made the same error with the frontend. So let me just go fix this, real quick...

Well, it didn't take long to fix. A solid minute, maybe? It does annoy me to have the folders called 'Backend' and 'frontend'. I'd *really* prefer it to be 'Frontend', but React doesn't agree. Ah well.

2024-04-22
-----------
I've now finished work on the add question endpoint. Controller class method, MediatR command, and service class method have all been set up. Time to try it out!

Ayo, it worked! After an hour and a half of debugging, that is. But hey, my endpoint worked flawlessly from the start. The problem was within my SQL database. So apparently there's something called an 'Identity column'. I'd no idea this was a thing. Apparently, if you create an `int` and call it 'Id', EF Core will configure that column as an identity column by default. And what is an identity column you (does anyone but me even read this?) might ask? Well, it means that one cannot manually insert a value into that column. Instead, the SQL database will automatically generate an Id, by default, starting from 1. For each created item, the Id will be x + 1. Sounds great, right? Yes, and it is, to be sure. Just not in this case. I'll probably add an option for deleting questions later. And this identity thingamajig doesn't account for deleted items - rather it will always be x + 1. Means if an item is deleted in the middle, that Id will remain gone. Which is not what I want. So, I had to fix that by changing the column from an identity column to a non-identity column. Buckle up...

Changing that column was *not* as easy as one might think. First off, I noticed the problem when I tried to create a question with a self-made ID. My poor database wasn't too happy about that. Only automatically generated IDs for identity columns. Anyhoo, I tried a bunch of stuff, researched quite a bit, and was sad when one solution I believed in didn't work. So eventually I settled for deleting the columns and recreating them. All I had to do was find a way to drop them. And that actually wasn't too difficult - just remove the related `DbSet`s from my DbContext file. Then, create a new database migration, update the database, take back the related `DbSet`s, create another database migration, update the database, and the tables have now been resurrected with the proper configurations. Obviously, I'd have to be a lot more careful if I'd had any data in the tables. Fortunately, I noticed this error early on. Means I didn't have to back anything up - there was nothing to back up.

So... All is in order now. My endpoint for creating questions works, my SQL database is back up and better than ever before, and I also adjusted the logic a tad to return a smooth response. The response for creating a question might look like this:

```json
{
    "question": "What is the chemical symbol for lead?",
    "option1": "Hg",
    "option2": "Pt",
    "option3": "Ti",
    "option4": "Pb"
}
```

It's a DTO of a question, so it doesn't include the answer that was submitted. I also adjusted some error handling and whatnot while I was at it, to make it a tad better all in all. The following link is the one I followed when dropping and recreating the tables: 'https://stackoverflow.com/questions/38909707/delete-table-from-ef-codefirst-migration'.

2024-04-23
-----------
Ahh, I'm tired. I've been trying to fix this one detail for two and a half hours straight. I want multiple question types. I want it. And it shouldn't be that hard to implement, but I want to see it done nicely. So I've been trying to make 'FourOptionQuestion' an abstract class that returns an instance of a subclass (one of the specific question types). And that's, uh, not going very well. It's going really not well, in fact. I'll continue on trying to fix this problem of sorts tomorrow...

2024-04-24
-----------
Alright, I've fixed the error above now. I got an idea today at work (internship) where I thought to save the question as a JSON object (a `JsonElement`, to be precise), and only deserialize it into a class *after* checking which type of question it is to be. Naturally, I ran into issues. And this sucker was not an explanatory one *at all*. It said: 

> System.NotSupportedException: 'Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported. 

To which I say: Absolute nonsense. The problem was the deserialization, sure, but it had nothing to do with no constructors or anything. The problem was that the class is an abstract class. Now, I thought about this earlier today, and myself noted that the 'deserialization into a class is what creates the class instance'. I knew full well that it would be impossible to deserialize something into an abstract class - one does not simply create an instance of an abstract class. Did I recall this at the time of writing the deserialization code? No. Did I remember when I got that error message? No. Instead, I sort of tunnel-visioned on the 'parameterless constructor' thing. So, it took me far longer than it should've to realize that the problem was that I need to deserialize the JSON into an inheriting subclass (I've set up some of these, for the question types). This, I knew all along, and noted to myself earlier today. Oh well. At least it works, now.

Alright, everything is now fixed, added, and up and running. I've verified in the database that every single question works as expected, and an appropriate error message is returned when an invalid question type is requested. I also rolled back the database once to combine some migrations into one, final migration. And I also dropped the databsae once. Let's say it was for fun, and leave it at that.

Alrighty! New endpoint added for deleting questions. And wouldn't you (me?) know it, the floating IDs idea worked perfectly! Well I got a few errors because I forgot to 1) Add the floating ID upon deleting a question, and 2) Remove the floating ID after using it. But hey, with those additions, it worked absolutely perfectly! If a question is removed, then its ID is stored in a separate table. And when a new question is created, that table is first checked for IDs. If one is found, it is used and then removed. If not, a new one is generated based on the length of the questions table. Perfect! Side note: I find it to be unreasonably fun to drop the database every time I need to add a new migration/verify table compatibility. I'll be sure not to do so when I actually add some meaningful questions...

2024-04-25
-----------
New endpoint added! I can now get any amount of questions (larger than 0, obviously) from the database with this new endpoint. If an amount of questions larger than what exists in the database is requested, all are chosen. Whenever a question, one or multiple, is requested, they are first randomized and then returned. It all went so well in fact, that I ran into a problem because I tried to prepare for it not going so well. So, I've previously created an AutoMapper map between a question and a DTO of a question. What I did not expect was for this to work even for lists of these questions. So I tried to add a map for lists of questions, and it crashed the program. Not immediately though, so it took a second for me to realize what the problem was. Anyhow, as I mentioned, apparently the first map was enough even for lists. So that's nice - no need for extra work this time, heh. What's next is probably to add an endpoint for verifying a user's submitted answer.

2024-04-26
-----------
Alright, I've been working on some small fixes for a bit now. First, I fixed a naming convention irregularity in 'QuizService.' Then, I changed the method in 'QuizService' for getting random questions from the database. So, my previous strategy was to get *all* the questions from the database, create a copy of that list of questions, randomize it, and pick X amount of questions. Not too efficient, in hindsight. Or concise. Both have changed - now all questions in the database are reordered (it doesn't affect the actual order of the questions in the database), and then X amount of questions are picked. This should be more efficient, and it is *far* more concise. And, I also added one more little feature of sorts. `JsonSerializerOptions` to the controller class, so that I can send CamelCase JSON instead of PascalCase. This should make the calls more consistent, since I'll be using React for it later. It did take me a while to get it working, though. I realized relatively quickly that I'd need to use Dependency Injection to get the options from `Startup.cs`. How exactly I was to do so, though, I was unsure of. I did figure I'd need to register some options with `.AddSingleton` to get them in the controller class, but I didn't get much farther than `services.AddSingleton<JsonSerializerOptions>`. So although I didn't quite figure it out on my own, I was definetely on the right track. The final result became as follows:

```cs
services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
});
```

I knew I'd have to set the `PropertyNamingPolicy` - I had code prepared for this. The `new` part was what I didn't quite figure out. But hey, I was kind of close, and I learned some more about DI!

Now, I'm going to attempt to slap the contents of `Startup.cs` into `Program.cs`. I know this is possible (I did from the start of this project), but I don't know how well this'll go. So, I'll commit and push any changes so that I can always roll back, should I manage to blow up the local repository.

Wouldn't you know it - worked perfectly! I followed some instructions, and now two classes, two files, are one. Looks rather slick, if I daresay so myself. I haven't noticed any problems in the API, so I'll assume it works as expected.

Now, for another slightly risky step - changing from SQL Server to SQLite. Why? Because it'll be infinitely much easier to manage between machines, and I know this will be necessary eventually. And also because I don't need a full SQL Server for this little project. Apparently, an SQLite database can (theoretically) manage up to like 9 quintillion questions. Don't think I'll need quite that many...

Ahhh... It works at last... I am tired. So, SQLite. In hindsight, I had to complete a few steps:
1. Set up a new environment variable for the SQLite database connection string.
2. Roll back the SQL Server database.
3. Delete all the migration files and snapshot.
4. Download SQLite NuGet package and remove SQL Server NuGet package.
5. Adjust from `UseSqlServer` to `UseSqlite` in `Program.cs`.
6. Create a new database migration.
7. Apply the new database migration.
8. Verify and potentially adjust CRUD operations.

If one knows what to do from the start (me, now...) this would be a quick and easy process. Get a load of creating the environment variable and then not restarting PowerShell and VSCodium. Was fun debugging that - I didn't get an error message that the "database wasn't found", instead, I got an error message that said that a certain table was missing instead. Eventually I restarted my computer, and applying the database update worked. Which is when I remembered that I have to restart programs that use environment variables after adjusting them. What else did I struggle with? The query that didn't work, I suppose. So I had some code: 

```cs
_quizDbContext
    .FourOptionQuestions
    .OrderBy(q => Guid.NewGuid())
    .Take(numberOfQuestions)
    .ToList();
```

This worked with SQL Server, but not with SQLite. I'm not exactly sure why, but it supposedly had something to do with the SQLite provider. Anyhow, slapping a `.AsEnumerable()` after `.FourOptionQuestions` fixed the issue. It's a tad less efficient, but whatever. It works, and it's still fast. So until I add 1000+ questions to the database, this difference in speed is negligible. And if I ever do (as if), I should probably go outside instead. So this whole migration from SQL Server to SQLite was a lot more painful than I'd imagined, but I've thought that many a time by now. So anyway. It works now, I've verified that all CRUD operations are successful. And I dropped the database once to check that it is successfully recreated each time I run `dotnet ef database update`. All is in order, once more. I may now sleep, now that it works again. I didn't intend to be programming until 22:53, but oh well.