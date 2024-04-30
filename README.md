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

2024-04-27
-----------
I've now added a query parameter to the get questions endpoint. So, one can either not include it to get X amount of any questions, or include a query parameter to get X amount of questions of the specified type. Now, I also added some code that returns an error if no questions of the requested type are found. This would happen either if the user misspelled the question type, or if no questions of that type yet exist. `git commit --amend` sure is useful for unpushed commits, indeed. Now, I'm considering how to continue. Either I add an endpoint for verifying submitted answers now, or later. I'll need to add endpoints related to quiz initialization/player statistics initialization, as well. There are a few things to consider...
1. Each player should be assigned X amount of questions of type X, when requested. IDs of these questions should be saved alongside the player's details in the `PlayerStatistics` table.
2. Correct answers should be saved in the same table as above - no frontend cheating will be allowed in these parts. When an answer is submitted, the related question's ID should be removed from the `PlayerStatistics` table, while the number of correct answers is updated (or not, if the answer was wrong).
3. I might want to add an endpoint for simply requesting a question, with player details and the submitted answer as parameters.
4. I'll need to add an endpoint for checking the player's submitted answer, and this endpoint should probably lead to step 2.

In my previous quiz project, I included player details and whatnot in the route. Not this time - I'll be creating a `PlayerDetails` or something class that'll hold all related information, and then create a POST method for it. It'll make the process a tad more clean, I should think.

Hm. I'm thinking of what to include in this `PlayerDetails` class. A name is a good start. Interesting - I just now realized that the `ID` property of the `PlayerStatistics` model is still a configured as an Identity column (it's autoincrement in SQLite, but eh, same thing). I could keep it that way, this time. I don't think I'll need to delete player entries that often, anyway. I could create an `InitializeQuizRequest` class and include some properties `PlayerName`, `AmountOfQuestions`, `QuestionType`, and so on. I think I'll do this...

Hm. I'll try configuring the relation between `PlayerStatistics` and `FourOptionQuestion` with a many-to-many relationship. Or one-to-many. I don't really know - I'll test some stuff and see where I land. I'd like to save the ID of several questions in each row of the `PlayerStatistics` table.

So I've been working with this whole many-to-many relationship whatchamacallit, and I don't really understand it. In the `PlayerStatistics` class, I have the following code: `public required ICollection<FourOptionQuestion> Questions { get; set; }`. However, I cannot see the questions directly in the database - which I assume is because another auto-generated table seems to be holding onto that information (sort of). But when attempting to reset the questions, I can't seem to be able to modify the `Questions` property (its value is null - so I'm getting the classic `Object reference not set to an instance of an object`). Something definetely seems to be amiss - I just need to figure out how to solve it. Tomorrow, that is. I am actually going to be a bit more reasonable today and not stay up until 23 PM.

2024-04-28
-----------
Alright! No more `Object reference not set to an instance of an object`. Apparently, I'd forgotten about a `.Include(ps => ps.Questions)` - and the questions were not loaded when I didn't explicitly ask for them. This is because EF Core uses lazy loading by default, yada yada. More efficient when you don't need questions, but I'll probably never be in that scenario. Unless I ever add an option for adjusting existing player profiles. In which case I should also probably go outside instead.

So, I've tested all the options, all the questions, and it all works well. I did notice one thing - if a question is removed, then it is also removed from every player's list of questions. While this is all well and good (it means that I'll never accidentally search for a deleted ID, sparing me quite some trouble), it does mean that each player will get -1 chances at increasing their correct answers. So, say you have two questions. One of them is deleted. Then you answer your first question. The quiz will assume you've answered all your questions, so your result will be 1/2 correct answers. It's now flawless - this could be improved. But honestly, I don't think I'll often delete a question. If anything, I'll probably PATCH it. And that *should* mean it updates for all the player objects in the `PlayerStatistics` table.

Sidenote: I kind of didn't commit any changes regarding new endpoints, so this commit's going to be about twice as big as it should be...

Okay, so I noticed a problem. When adding questions to the `PlayerStatistics` table, despite first randomizing the questions, I would always get them in the first, ascending order when fetching them. . So this was a big problem. One I solved by adding the randomization to the method for getting questions. Did it work in the moment? Absolutely. Was this a bad idea? Absolutely? Did it come back to haunt me, 20 minutes later? Absolutely. So, if I randomize the questions when *fetching them*, how can I determine which question to target when checking checking the user's submitted answer? That's right, I can't. Unless I let the user send the question ID themselves (as if - no cheating!). So I remade added a join entity myself (the previous one was autogenerated) between the `PlayerStatistics` table and the `FourOptionQuestion` table. It made the whole ordeal more complex, but now I can save an order in which the questions are to appear - which means I can keep the random nature of the questions. So, now it all works! I can now initialize the quiz with a player name with an amount of questions and potentially a question type. I can also re-initialize the quiz if I want, which resets the properties of that player entry in the `PlayerStatistics`. And, I can get a question from the `PlayerStatistics` table, which now maintains a random nature.

And it's all working! From what I can tell, everything's now up and running flawlessly (with perhaps some minor exceptions that I've yet to find). I believe I've already explained what is possible, so I'll skip that. I did add a new endpoint, however, for checking one's submitted answers. A string "Correct!" or "Incorrect" is returned. If the answer is correct, then the variable holding the number of correct answers is updated by +1. If one requests a question when they have none left, a message is returned with an `OK`. If one tries to check an answer when they have no questions left, a message is returned with a `BadRequest`. I think this should suffice for all regular purposes, but take that with a grain of salt - I'm not certain. So, next up's the endpoint for adjusting questions: HTTP PATCH.

Alrighty! PATCH endpoint added and functional. It even updates questions that are already added to any initialized quizzes! Next, I'm not sure if I should add anything else to the backend, so maybe it's time to start frontending?

2024-04-29
-----------
Right, unit tests exists. Not that I mind - I'll take any excuse to postpone the frontending. So I'll start writing some of these unit tests. If I'm going to test every scenario in the controller, mediator classes, and the service class, this is going to take a while. But hey, the longer it takes, the longer I can delay the frontend. More backend tests, less frontend. Win-win!

Alright, first test's up and running. Kind of. I'm having some trouble with a null-related problem regarding the response of the method I'm testing in the controller class. I have some code `response.Result.Should().NotBeNull()`, which means that if `response.Result` *is* null, the test will fail. So `ObjectResult objectResult = response.Result as ObjectResult` is fine. If `response.Result` is null, the code won't reach that part. But alas, the compiler doesn't agree. So now I'm stuck with a null-related error that I'll have to try fixing tomorrow. But hey, at least the tests project's working, so there's that.

2024-04-30
-----------
So apparently `ObjectResult objectResult = (ObjectResult)response.Result!;` works to remedy the null-related warning, despite `ObjectResult objectResult = response.Result as ObjectResult` not working. I actually came up with this solution yesterday, about a minute after pushing the commit. So I didn't spend a lot of time fixing this, for once. However, this caused me to notice that apprently the JSON is invalid, or something. So the test is failing now that I'm trying to run `objectResult.StatusCode.Should().Be(200);`. So, I'll get to fixing that... If I'm to make a wild guess, I'd assume it's because of a mismatch in title case. I think I'm sending `"Question": "My question"` and not `"question": "My question"`. Allow me to verify...

That was it, alright! A mismatch in title case (is that even what it's called?). And hey, good thing I brought Dependency Injection to the table, because I already had the `JsonSerializerOptions` prepared! All I had to do was add `options`, which I'd already gotten through DI, to the serialization part. And just like that, the test now passes.

I've now added the remaining assertions, and it all went according to plan. So, the first test is up and flawlessly running (I think?). Now, time to get the ball rolling...

Well hey. Seems these tests sure have proven far more fruitful than I'd imagined. Already have I solved several minor issues. And I just now noticed a less *minor* issue. Take a look:

```json
{
    "question": "What is Eyjafjallajökull?",
    "option1": "A glacier in Norway",
    "option2": "A volcano on Iceland",
    "option3": "A crater in China",
    "option4": "A city on Greenland",
    "correctOptionNumber": -10
}
```

The correct option number is -10 in the example. Absurd right? No quiz would ever accept that. Except mine, apparently. So, let met just go fix that, real quick...

Hm. It was not as easy as I'd hoped. I was hoping to add a [Range] attribute and leave it at that, but alas, it was not that easy. Attributes like these are not inherited, so it quickly became a *lot* more complicated. I tried marking the property in the superclass `abstract` and `virtual`, to override it with a [Range] attribute in each subclass. It did not work. I tried modifying the getters and setters like this:

```cs
private int correctOptionNumber;
public override required int CorrectOptionNumber { get { return correctOptionNumber; } set { if (value >= 1 && value <= 4) correctOptionNumber = value; else throw new ArgumentOutOfRangeException(nameof(CorrectOptionNumber), "Value must be in the range 1-4."); ; } }
```

Incredibly readable, I know. Anyhow it did work, but not as I want it to. Throwing an error like this means returning an Internal server error, which is not what I want - I want to return a `BadRequest`. So now I'm experimenting with ways to validate the JSON before deserializing it.