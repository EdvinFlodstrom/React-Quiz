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

So, I've tested all the options, all the questions, and it all works well. I did notice one thing - if a question is removed, then it is also removed from every player's list of questions. While this is all well and good (it means that I'll never accidentally search for a deleted ID, sparing me quite some trouble), it does mean that each player will get -1 chances at increasing their correct answers. So, say you have two questions. One of them is deleted. Then you answer your first question. The quiz will assume you've answered all your questions, so your result will be 1/2 correct answers. It's not flawless - this could be improved. But honestly, I don't think I'll often delete a question. If anything, I'll probably PATCH it. And that *should* mean it updates for all the player objects in the `PlayerStatistics` table.

Sidenote: I kind of didn't commit any changes regarding new endpoints, so this commit's going to be about twice as big as it should be...

Okay, so I noticed a problem. When adding questions to the `PlayerStatistics` table, despite first randomizing the questions, I would always get them in the first, ascending order when fetching them. So this was a big problem. One I solved by adding the randomization to the method for getting questions. Did it work in the moment? Absolutely. Was this a bad idea? Absolutely? Did it come back to haunt me, 20 minutes later? Absolutely. So, if I randomize the questions when *fetching them*, how can I determine which question to target when checking checking the user's submitted answer? That's right, I can't. Unless I let the user send the question ID themselves (as if - no cheating!). So I created a join entity myself (the previous one was autogenerated) between the `PlayerStatistics` table and the `FourOptionQuestion` table. It made the whole ordeal more complex, but now I can save an order in which the questions are to appear - which means I can keep the random nature of the questions. So, now it all works! I can now initialize the quiz with a player name and an amount of questions and potentially a question type. I can also re-initialize the quiz if I want, which resets the properties of that player entry in the `PlayerStatistics`. And, I can get a question from the `PlayerStatistics` table, which now maintains a random nature.

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

Hm. It was not as easy as I'd hoped. I was hoping to add a [Range] attribute and leave it at that, but alas, it was not to be so easy. Attributes like these are not inherited, so it quickly became a *lot* more complicated. I tried marking the property in the superclass `abstract` and `virtual`, to override it with a [Range] attribute in each subclass. It did not work. I tried modifying the getters and setters like this:

```cs
private int correctOptionNumber;
public override required int CorrectOptionNumber { get { return correctOptionNumber; } set { if (value >= 1 && value <= 4) correctOptionNumber = value; else throw new ArgumentOutOfRangeException(nameof(CorrectOptionNumber), "Value must be in the range 1-4."); ; } }
```

Incredibly readable, I know. Anyhow it did work, but not as I want it to. Throwing an error like this means returning an Internal server error, which is not what I want - I want to return a `BadRequest`. So now I'm experimenting with ways to validate the JSON before deserializing it.

Ahem.  
> 'Validaiton failed: Correct option number must be in range 1-4.'

*WOOOOOOOOOOOOOOOOOOOOOOOOO*

Anyway, where was I? The validation works. Hardly have I ever before been so exited to see an error message. It took me a lot of time and a lot of thinking to get this working. So although the validation doesn't occur before deserialization, it occurs before sending the MediatR command. Which now means I can return a `BadRequest` immediately when the user sends something dumb like `"NumberOfCorrectOption": -1337`. And this also means I don't have to bother fixing an attribute for the inheriting subclasses of `FourOpionQuestion`. A foul value can be set, but it won't get much longer than the deserialization part. However, since I moved all the subclasses and such to a new `Infrastructure` folder, I can no longer use `git restore x` to fix it instantly. So, time for some good old manual code cleanup...

Hookay, all's fixed now. I've renamed, refactored, re-read, re-tested, and everything some more. I believe that I've, after a *lot* more work than I'd expected, gotten the `CreateQuestion` controller method just perfect. Any potential mistakes, intentional or not, should be caught. A warning is logged, and a `BadRequest` is returned. At least for all the cases that I've found out about. It's funny how I, prior to starting work on these unit tests, thought the `CreateQuestion` method was fiiine, it was good. No need to work on it or other parts of the backend... Yeah that was wrong, to say the least. Safe to say, I didn't think there were so many things that could be improved in my code. Which worries me, because I currently think "it's fiiine...". Which it probably isn't. But, uh. At least it's a bit better than before?

So, apparently, the validation wasn't *quite* functional. Apparently one could still create questions with one or more empty options. So, I had that fixed. Now, I *think* the `CreateQuestion` endpoint in the controller should be basically perfect (sure). But, uh, I have a feeling one can still PATCH a question to have invalid properties. So, that's what I'll be fixing next...

2024-05-01
-----------
Well, I wasn't completely wrong. Here's a PATCH request:
```json
{
    "question": "",
    "correctOptionNumber": 2
}
```

It is supposed to fail, but it currently doesn't. I'll have to find a way to fix this - probably with custom validation. Any property should able to be null, but not empty. One nice thing is that I can't set the correct option number to anything outside the range 1-4, but I can leave it out. So I'll simply have to make sure that no property is set to an empty value... In theory, all that's required should be to set a rule for when it isn't null, I think?

Hey hey, it's working! Any one of the options may be null, but if any isn't, it may not be empty. So to change one option and leave the others unchanged, simply include only the one option to change and provide a not-empty string.

I don't think I need to add validation for HTTP DELETE? It's just an integer - if it doesn't exist in the database, that's returned as a `BadRequest`. Suppose I'll start writing tests for HTTP PATCH and DELETE then?

Okay, I've now improved the error handling in the `CreateQuestion` method even more. Previously, if JSON of the to-be question was null, the deserialization method threw an Exception that was caught and returned with a special message. This meant an Internal Server Error, though. And this bothered me, since it's more of a `BadRequest` to send invalid JSON, yah? So I fixed that, and now a `BadRequest` is returned for bad requests. Makes sense, hm.

Huh, I just now noticed a curious problem. One can set the ID of the question in the request. This doesn't actually matter - I have logic later in the program that sets the ID every time, so I could tehcnically ignore this. But admittely, it does annoy me somewhat that it's possible in the first place. So, I'll see what I can do about that...

Alright, problem above's been fixed. It really wasn't as confusing as I made it seem, because I (somehow) edited the wrong property. I don't know how I didn't realize I was editing the wrong property for about five to ten minutes, but when I noticed, I simply added the code to the right property instead. I dropped the database about seven times and tested a bunch of stuff, and now I'm pretty sure it works as I want. Now, even though the ID of the question could not be tampered with from the start, it can't be assigned at all anymore. I also added this thingy to another property, that users should not have access to...

Hmkay, tests for DELETE have been added now as well. So, I have a total of 12 functional tests now, for `CreateQuestion`, HTTP PATCH, and HTTP DELETE. I suppose I'll keep working on unit tests for the remaining endpoints, then.

15 tests are up and functional. I'm starting to get a little tired of tests though, so I'll take a break for now. Not quite done with the testing yet, though...

Alright, some extra validation has now been added to the `InitializeQuiz` endpoint. Now, one may no longer provide a name that is shorter than two characters long. If your name is "" or "A" or something else with only one letter, sorry. Got to keep the names unique, yah?

I've now updated the validation above. Suppose I forgot to think before adding it, cause I made the validaiton only for `InitializeQuiz`, which inherits from `BaseRequest` (not very explanatory name perhaps, but oh well). This `BaseRequest` class is the one containing the `PlayerName` property, so I adjusted the validation to be of type `BaseRequest`. As such, I can now reuse this same validation not only for `InitializeQuiz`, but also for any other methods that implement a form of request that inherits from `BaseRequest`. By a complete coincidence (for the sake of clarity, this is a joke), this is the case right now. So I'll be reusing the validaiton for the `GetQuestion` method. Also, I feel inclined to point out how useful the factory class I made for this validation has proven. I can validate the a subclass with the validaiton methods of the superclass like this: `(bool success, string? validationMessage) = ValidateRequestAndLogErrors<BaseRequest>(request);`. So despite `request` being an instance of a subclass, I can still validate it using the superclass' validation. Polymorphism sure is useful, eh? Also, this is what the method declaration of the method in question looks like: `private (bool success, string? validationMessage) ValidateRequestAndLogErrors<T>(T instance)`. The `<T>` is real nice, indeed. Means I can specify to validate using the superclass type instead of the subclass, which is chosen by default unless I specify the superclass type when calling the method.

Hm. I just noticed an error. A rather problematic one, one could say. So, the `GetQuestion` method returns a `Task<ActionResult<FourOptionQuestionDto>>`. And this is a problem, because when there are no questions left, I instead return a string. A string. Good luck treating that FourOptionQuestionDto as a string, me. So I'll have to adjust this logic, somehow. I just don't know exactly how - there are many ways to do so...

Huh. Now, something like this can be returned when asking for a question:
```json
{
    "fourOptionQuestion": {
        "question": "What is Eyjafjallajökull?",
        "option1": "A glacier in Norway",
        "option2": "A volcano on Iceland",
        "option3": "A crater in China",
        "option4": "A city on Greenland"
    },
    "details": null
}
```

And if no questions remiain:
```json
{
    "fourOptionQuestion": null,
    "details": "That's all the questions. Thanks for playing! Your final score was: 1/1. To play again, please initialize the quiz once more."
}
```

I guess this is what I was technically after, since I can now check "Is question null? Render the details instead". It doesn't look as slick in Postman, but I suppose that's not what I was after anyway, so what do I know? I'll leave it like this for now, since it'll make the tests a lot more comprehendable (relatively speaking, that is).

Hm. I've written many a test today. Well and yesterday. In about two days, I've written 22 functional tests. 825 lines. Yeesh. Only one controller method remains to test now - `CheckAnswer`. And then I'll have to test all the MediatR commands. And all the `QuizService` methods. This is going to take a while, eh?

2024-05-02
-----------
Alright! All the controller tests are now done. Maybe I went overboard, I don't really know. 909 lines of tests is a bit. But each controller method is tested in at least three ways (the more complex ones (such as `CreateQuestion`) are tested even more), so at least I've covered most cases. Now, I suppose I'll write some MediatR command and `QuizService` tests?

Alrighty, first test's up and running for the `CreateQuestionCommand` MediatR command. I did run into a bit of a problem when I needed to mock `QuizService` for the test. I recall hearing that it's always far easier and generally better to mock an interface rather than the class directly. So, I added `IQuizService` and replaced `QuizService` with it in every related class. And I actually realized that the current registration of `QuizService` would need to be adjusted in `Program.cs` with this new interface class. So I fixed that before getting an error, and hey, it worked first try! So, with this new interface class, I can easily mock `IQuizService` for the tests. I'll create more of these unit tests tomorrow.

2024-05-03
-----------
All the handler unit tests have now been written. These ones clocked in (wrote in?) at 691 lines. All of them pass, and I'm pretty sure they all work as intended. Each test contains around 3-8 assertions, so I'd be very surprised if any single one of them passed in a way I did not indend. Anyway, now it's time for what I can only imagine as the most challenging of these unit tests. `QuizService`. This is going to be a hefty testing file.

Hm. Already I've run into a problem, and I only just created the file. The problem I previously postponed. `QuizDbContext`. I have barely any clue how to mock a database context, let alone mocking the entire database. Hm.

2024-05-04
-----------
Alright! The basics are now set up. I've yet to write a full, test, but I've verified that the `QuizDbContext` instance and that an in-memory SQLite database both appear functional. Something to note is that all tests share the same `Mock<ILogger<QuizService>>` and `Mock<IMapper>`, but don't share the same `QuizDbContext` and `IQuizService`. Each test gets its own instance of these two, so that any database interaction in each test stays unique to that test - an item added to the database in one test should not mean an item is added to the database in another test. And it works! From what I can tell, this is because each SQLite in-memory database thingamajig is destroyed once it exits the scope. And the scope in question is (if my understanding is correct) each unit test. So a small, resource efficient database is created and destroyed for each test. I thought it sounded inefficient at first, but it seems like this is the usual way to do it. So I'll roll with it and see where it takes me.

Hohohoh, the first test is functioning just as planned! It's successfully tested that a question is created and added to the database, this one when there is a 'floating IDs' present. The next test shall test what happens if there are none present.

Worked flawlessly! So, I think I know everything I need to know about unit testing the service class now. So, time to get the ball rolling...

Hm. I noticed that I cannot manually insert a question's ID into the in-memory database using these unit tests. This is because of how I've handled the setting of the ID property. It's not a problem, *yet*, but it's worth noting for the future - as it may or may not become a problem later.

Hm. There are more test cases I could cover for some of these methods, but some of them are *probably* not necessary. For example, for PATCH, the `PatchRequest` will never be null as I've added error handling for that in the controller class. So, I'll probably mainly focus on the major test cases for the service class.

Heh, guess what's come back to haunt me? That's [probably] right: The unability to manually set question IDs. For the `GetManyQuestions` endpoint, naturally I'll want to add multiple questions to make sure they can all be retrieved at once. Aaaand as I figured, that's not possible. If I don't set the ID property, it's default value will be 0. Which means trying to add multiple questions is a quick ticket to an Exception. Time for some testing...

Hmkay. Problem above should be solved. The solution I used was one that I've actually experimented with previously, adding `[JsonIgnore]` to the property. Means I can set it in the code, but JSON being deserialized won't affect it. I think this is a good solution?

Okay. More tests have been added. I know I previously stated that I'm not complaining about doing unit tests rather than frontend, but I can't deny that I'm starting to grow a tad tired of unit tests now. I think I've written more than 1700 lines of unit tests in the past three days. Thing is, I know I've been copy-pasting a lot of arranging, but I was pondering whether or not that is the intended way to do it. I could add methods for preparing the set ups, but might that make it less clear *exactly* what is being tested and included in each test? Not sure. But I digress, I have tested every method in `QuizService` but two. The remaining tests will be written either later today or tomorrow.

2024-05-05
-----------
Huh. I just noticed something curious in the method that converts the player name to title case. Something like "åke åkmaN" becomes "Åke Åkman" as expected, but "åke ÅKMAN" becomes "Åke ÅKMAN". Interesting. To keep it consistent, I think I'll convert to lowercase before converting to title case.

I solved the above issue by converting the player name to lowercase before the title case thingamajig. Perhaps not perfect, but I don't really care. While I'd like this quiz project to be playable, I don't feel like deep diving into the strictest areas to enfore *exact* rules. At least not everywhere. Also, the tests are done. So far, anyway. Perhaps I should've written more tests for `QuizService`, since I'm only testing what happens when a `DbUpdateException` is thrown in one of the tests, but the error handling regarding that is the same in each method anyway. So, nah. Maybe another time. 61 tests in total, at 2541 lines. Got to thank `git ls-files | xargs wc -l` for that information. When I run all the tests simultaneously, it takes about 1.5 seconds. So that's pretty good, I should think. 

Well, I suppose it was bound to be inevitable at some point. I do believe it's now time to start frontending. As much as I prefer backend development, I do realize that the quiz isn't exactly playable without some form of reasonable UI...

Hmkay. I've now prepared 40 more question creation requests in Postman, making for a total of 50 that I can create at any time. There's probably a more efficient way to do this (and once I'm certain I won't change anything with the database, I'll back it up so I don't have to manually add all the questions on occasion), but it works. I also added some question creation formatting. Should you forget a '?' at the end, it'll be added automatically. Nothing too fancy, but, again, it works. Funny how a project that's supposed to be 50/50 backend/frontend is currently 97% C#. Alas, I fear this is soon to change. I should probably stop postponing the frontending and get to it at some point. Might as well be now?

Ooookay. Some time later, I've now created something. Just barely, but I still feel like it's worth noting, because I SUCK at React (or, well, frontend in general). So. When the page is first loaded, you are welcomed with a welcoming page. Makes sense, hm. So, it says "Welcome" this and that, and "Below, you can find your options". Below, you can find your options. Currently consisting of a single button, "Take Quiz". But, what made me feel a tad cool, is when you click the button. Then, a new component is loaded. So, the text and button are replaced with some new text, currently, "Initialize the quiz...". This is the component I'll keep working on. And I should probably add some CSS soon. If I'm to have a GUI, it ought to look a tad nicer, at least.

Hmkay. I've now worked for a little bit on CSS. I'm bad at frontend in general, and I'm *really* bad at CSS. I can't do colours or style for the life of me (probably because I avoid it at all costs). I like my simple CLI projects that don't require any fancy styling (I do realize that you can achieve a lot with CSS, I just don't feel like prioritizing learning it). But anyhow, the welcoming page looks a little better now. Just barely, though. I'll have to work a lot more on it. But hey, progress is progress! I'll keep working on the frontend tomorrow.