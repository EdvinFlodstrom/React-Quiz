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
    "option1": "A mountain range in Norway",
    "option2": "A glacier in Iceland",
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
        "option1": "A mountain range in Norway",
        "option2": "A glacier in Iceland",
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

2024-05-06
-----------
Quick backend detour: I renamed a file (two, but one's an interface) from `QuestionValidatorFactory` to `QuizValidatorFactory`. To make it a tad more clear that the validator isn't exclusively for question validation. So, back to React. Or CSS, rather. I need to fix this atrocity of a welcome page...

Ayo, it actually looks pretty good now! Sure, it's far from perfect. But can a project ever be perfect? Anyhow, a linear gradient, some shadows, new colors, and suddenly it looks a lot more welcoming, I should think. I hesitate to admit that it wasn't even boring. It was actually kind of fun in a way to experiment with different colors, ideas, and such that would make the result look nicer. So, I think the welcoming page should be fine for now. I'll most definetely work more on it later, but tomorrow I shall work more on the React components regarding quiz initialization.

2024-05-07
-----------
Hm, so I've added an animation of sorts to the background gradient. When a button is clicked (only specific ones), the gradient moves. I had to increase its size for this to be possible, so the background looks a little different now. No matter, I still think it looks fairly decent. And, I'll say, I really like the animation. When the function for moving the gradient is called, it is either moved, or moved back. So, it works both ways without a lot of extra struggles. Nice. Also, I can pass this function to subcomponents, to have buttons in those components cause this gradient animation, as well. Seems work on the React components regarding quiz initialization will continue another time, hm. Maybe tomorrow?

2024-05-08
-----------
I've refactored some code, added some CSS, and tried for about an hour to implement a form. It's not very fun. So, I got an error message that I was trying to update something from an uncontrolled value to a controlled value, or something. This was really dumb, for reasons. So, it was for the form, with a player name, amount of questions, and a nullable question type. The amount of questions input thingy got upset regarding the issue I just mentioned. So, in the end, I had to create a separate `useState` whatchamacallit just to force the number of questions thing never to be 0 or an empty string. Which I'm pretty sure was already the case, but whatever. And now I've run into an issue with React being React. I'm updating the form data (it's a variable that holds the player name, number of questions, and question type), and then using what's in that form to enable or disable an `Initialize Quiz` button. Except it's not working because asynchronous this and asynchronous that. The data in the form variable doesn't update immediately, rather, it does so after the next React update thingamajig. I don't really know or understand it. My backend brain isn't used to having to wait for stuff to update other stuff just to use stuff. So, time to find a workaround for this, too, I suppose...

Hmkay. Seems to work just fine. When the data in the form is invalid, the `Initialize Quiz` button is red and disabled. When the data is valid, the button is enabled.

Okay. I don't know quite how much time I just dedicated to this timer. I don't even know where to start, to explain this absolute bane of my existance. I'll start by saying that it currently seems to work. So, it's a visual and logical timer. No boring numbers saying how many seconds you have left. This is a visual bar that depletes over 15 seconds (hard-coded both in JSX and CSS. Means if one is changed, the other doesn't reflect it. Not good, but I really can't be bothered to fix it right now. Not that I know how to, for that matter). So, when I tried to fix the visual and logical parts at the same time, things got confusing. Whenever the component (`TakeQuiz.jsx`) loaded, the timer would instantly start ticking down. Didn't help that little ChatGPT told me that all code within the component is executed once the component mounts. Which was absolute nonsense. So, the timer didn't *actually* start ticking down. Only the visual bar did. And that had me real confused for I don't know how long. Eventually I got that figured out, only to run into more issues that I've managed to forget about already. But I think it took me well over 2 hours to fix this single timer bar thingamajig. And I never want to do it again.

So, a POST request has now also been added to the backend. Not sure why I didn't do this earlier, since it's to be mandatory to make this request before taking the quiz. Ah well, doesn't really matter. It actually went fairly well, adding this POST request. I know for a fact that some frontend fellas would complain at me for putting a plain request like this in the component and not have a separate file/component thingy where I store all the requests. But yeah, no, not going to do that. Yet, anyway. Too much new stuff for me, for I am a simple backend bozo. Also, I slapped some error handling on that POST request as well, so if the request goes poorly (either because I forgot to boot up the backend, or because the player made a mistake such as requesting an invalid question type), a red error message appears below the button for initializing the quiz.

Did I mention that I added an animation for the `TakeQuiz.jsx` `<h1>` element? Well, I did.

2024-05-09
-----------
Alright, I've now added some buttons, some CSS, and a POST request for getting a question. I also made sure to pass the player name as a prop from `InitializeQuiz.jsx` to `TakeQuiz.jsx`, so that the POST request works (it requires a PlayerName property, since it needs to find the player in the SQLite database). So now I can click a button to get the question, which is instantly displayed, then the program waits for 3 seconds and then displays all the options whilst having the timer start ticking down. I'll continue by adding a function that each button calls (with a different number, 1-4 based on the button) to make a POST request check the answer. I've already written the code for stopping the timer, and I think it works. No non-JavaScript-promises though.

Hmkay, another quick backend detour later, and now the `CheckAnswer` method returns a `CheckAnswerResponse` object (containing a string and a bool) instead of just a string. Although not strictly necessary, I believe this will make it easier/more concise and clear in the frontend when I check if the answer was correct or not. Means I only need to check a bool for true/false instead of comparing strings.

Hm. I've run into a bit of a roadblock. So, I'm currently implementing the logic for checking the submitted answer. It works, and it's basically done, however, there's one tiny, major issue. The only way to progress the questions, to get another one, is currently to check the answer. And to do that, the only thing the API accepts is a number in the range 1-4. Sounds like it would usually be good, I think. But, what if the timer runs out? Then the answer is wrong, no matter what. But to progress the questions, an answer in the range 1-4 must be provided. There are frontend workarounds for this, but that won't suffice. It's the backend that keeps track of the correct answers. So, I'll have to deal with this issue in the backend. Somehow.

Hmkay. The above issue wasn't too difficult to solve, after all. I simply added 0 as a valid question answer. Obviously it'll always return incorrect, but I suppose this should be a fair-enough way to solve this issue. After all, if the time runs out, it doesn't matter what the player was going to answer.

Okay. The quiz appears mostly playable right now. I want to add something that shows what the correct option was, but I haven't figured out how to do so just yet.

Right, flashing animations for the buttons have now been added. If you selected the correct option, only it flashes green. If the time runs out,only the correct one flashes green. If you select the wrong option, it flashes red and the correct one flashes green. I fear that not only my JavaScript, but also my CSS, is kind of spaghettified. It's not looking too good. But hey, it works. So that's something. I'll consider what to do next. Also, I had to change the backend a tad again, to return the correct option number for the button flashing thingamajig. Previously, if the player selected the wrong option, the frontend would have no idea what the correct option actually was. So that's fixed now.

2024-05-10
-----------
Hm. Never thought I'd see the day that I'd willingly delve into CSS to try to troubleshoot responsiveness in my application. But you never know, I suppose. So anyhoo, I started by 'fixing' the background flashing animation for the `TakeQuiz.jsx` component. Like a true CSS professional, I added the following code: `padding-block-end: 8rem;`. No need to tell me how ugly of a solution that is - I'm fully aware. But hey, for my purposes, it works. So now the flashing animation covers the entire screen. Also, I blame React for making me go with this solution in the first place (ha!), since I wanted to add the class for the flashing animation to the body. Alas, this did not work. I added the `<div>` as far up as I could in the chain (at least I think I did?), but it merely covered all the other content - not the *entire* page. So, although not perfect, my solution appears functional. I think. Can't be certain though, heh. Also, there was another issue...

So, this other issue was that, in `TakeQuiz.jsx`, the grid (for the four options buttons) seemingly wasn't acting as intended. When the screen size wasn't wide enough, the buttons no longer lined up in the center as they should - instead, they moved to the right. It took me a few minutes (not that long, maybe 10-15 minutes?) to figure out the core problem. And it had to do with the padding and margin that the buttons 'inherited' from the other button class. These option buttons have two classes, the default 'button' class and a special button class for quiz option buttons in particular. So, I reset the padding and margin in these classes. And, since Crying Style Sheets also has a cascading feature, the reset margin and padding overrode the previous values (as intended). And this fixed the issue, so now the buttons and the grid accurately adjust based on the screen size. Also, the size of the buttons scales up or down to cover their respective 'place' in the grid, so that's why I could remove the padding and still have them be large.

So, I was essentially debugging CSS for 40 minutes straight, and it wasn't even boring. Which fascinates me, because I typically don't like CSS. I most certainly wouldn't have said the same, if I'd lacked access to the Firefox debugger. That one's real good, I think. So, with this done, I think I'll start working on a new component for creating questions.

I've now slapped on a check for the `CreateQuestion` endpoint in the backend. Now, you can no longer create the same question twice. And, I feel inclined to write down this little thing that happened. So, yesterday (or maybe it was two days ago? I don't remember), when I wanted to get some feedback on the application, I added 25 of my 50 stored questions to the database. Or so I thought - I actually only added 24. This I realized after receiving feedback. And now, today, when I wanted to test this new check, I went to add another question. I believe I picked one at random, although I don't recall for certain. And huh. The question creation didn't fail. Wouldn't you (me?) know it, I apparently picked the one question that I'd missed yesterday (2 days ago?). So that had me a tad confused for a second. Then I tried creating the same question again, and boom. 400. I also tried creating another question that I knew was already present (because I verified its existence), and boom, 400. So, that's nice. Now, I'll write a quick test for this...

Alright, the test is now up and running. It's all working as expected, I think.

2024-05-11
-----------
I just noticed a *big* problem with the little web app. So, for fancy effects and whatnot, I've implemented a gradient that is several times the length of the screen in width. Probably about 4-6 times as long. This allows for a sliding animation that makes for a cool effect. What took me over a week to notice, however, is that the user could scroll to the right to see this never-ending gradient. So that wasn't good. I thought I was doomed for a moment there, but apparently there was a relatively easy fix. Just slap `overflow-x: hidden;` on the body, and `user-scalable=no` to the 'index.html' file. And boom, it works. I'm still trying to figure out how to set a minimum width of the various elements though. I'm not sure why `min-width` isn't working.

Hm, I've now also spent a little time on making the rest of the page look a tad nicer on smaller resolutions/screens. I used `@media screen and (max-width: 600px)` as the tool for checking the minimum screen size, and it seems to work well. I haven't tested all that much, and I won't, because even though it would be cool to have a responsive web application like this, I'd basically be willing to wager money on the fact that no one will actually play this quiz on a phone. The primary reason for this is because the application relies on the backend being hosted locally. And I doubt anyone will try to set it up on their phone. I certainly won't. But yeah, responsiveness, woo!

Well. Only now did I realize a problem. So, a while back, when I wanted to stop the user from manually entering the ID of a question (it doesn't matter even if they do - that ID is going to be ignored), I added `[JsonIgnore]` to the `Id` property. This means that when deserializing into JSON, the property is ignored. What I failed to notice, however, is that this doesn't just apply to deserialization. It also applies to serialization. So I can no longer get the ID of a question using the `GetManyQuestions` endpoint - the only way to get a question ID is to check the database. Or set a breakpoint in the application. Both are workarounds, as I'd rather have the ID being returned with any endpoint using `FourOptionQuestion` instead of `FourOptionQuestionDto` (the DTO doesn't include the correct answer, the ID, and one other property). I'll see what I can do about this...

Eh, whatever. No more [JsonIgnore]. It was never necessary in the first place, as proven by the fact that sending an ID of 1000 still means the question's ID is the count of existing questions + 1. So, now I can get the ID of all questions using the `GetManyQuestions` endpoint. I won't use this specific endpoint in the frontend, since it returns all the correct answers, but maybe I'll use a variant of it that returns only IDs? We'll see...

Hey, I actually utilized React's capabilities for once! I've now created a component `QuestionForm.jsx`, that I'll use both for creating questions and modifying them. So, that'll make the code a lot more concise in those areas. Now, I'm fully aware that the application should be split into about a billion other components for reusability this and reusability that. But nah, this'll do. I know I won't expand this project a lot when it's done, anyway.

Huh. I've run into a curious issue. The JSON deserialization is failing because it expects an int, but receives a string. Took me a bit to realize something interesting, though: If I don't change the correct option number and try to create the question, it works. If I change the correct option number, it won't work. So, with some JavaScript magic, the int (sorry, *number*) is converted to a string. I'll see what I can do about it.

So, the default correct option number is 1. If I change it to 2, it doesn't change from 1 to 2. It changes from 1 to "2". So that's cool.

Okay. I've solved the issue now. To be frank, I really don't understand why it was an issue to begin with. The logic related to the int (sorry. **Number**) is identical between this component and `InitializeQuiz.jsx`, so it *should* work. The input in the form is set to `number`, just like in `InitializeQuiz.jsx`. So, I don't get it, but whatever. By converting it to an int, it works now. So if the request is successful, the gradient moves and you get a 'Success!' message (it's at the bottom). If it fails, you get a '[insert error message here]' message, and no gradient movement. Naturally, the two have different colours. Also, the duplicate question check works, so you can't add the same question twice. So, I think that's the `CreateQuestion.jsx` component done?

I've now added even more responsiveness to the text, both regular `<p>` tags and to the text in `<form>`. Again, this is not quite necessary, but if you think about it, CSS isn't exactly necessary either. CSS makes it look better, and that's why it's worth doing. So, I suppose that's why I'm adding responsiveness as well. Got to have it looking nice, yah?

Hm, so I've been considering for a bit now how to implement the question modification thingamajig in the frontend. And I think I've come up with an idea that should suffice. First, I'll create a new backend endpoint for getting a question by ID. So, my idea for the frontend part is this: 1) Input a question ID in a form and click a button to ask for a question. Then, that question and all its alternatives appear in a form (`QuestionForm.jsx`) below, where the user can edit them as they like. The ID should not be editable, and thus, not be returned by the new backend endpoint. Additionally, I'd like to try a bit of test-driven development, so this time, I'll write the tests first.

Hm, no. I'm probably doing it wrong, but test-driven development doesn't seem like my cup of tea. Mostly because I'm getting errors because I haven't yet created the classes, which I assume it's actually the intended development way. Oh well.

Alright, the tests are now up and running. What was it I added, like 7 tests I think? Nothing too crazy, because the endpoint is rather simple. Either a quesiton exists or it doesn't. Not too much can happen in between that.

2024-05-12
-----------
Hm, so I don't have much time today. But I've successfully created one more little component, `GetQuestionByIdForm.jsx`. I'll use it for fetching a question when modifying questions or deleting them. So that people can verify that "hm, this is the one I want to remove, yes" before unknowingly annihilating the wrong one. Next up, which will be tomorrow (maybe later today, but probably not) is to move some logic from `CreateQuestion.jsx` to a new component. This is to keep the project DRY. I *can* copy paste a lot of the code, but it hurts to do so. Even though it's frontend, I'd rather try not to make it too ugly. It's React, after all. Might as well use component functionality and compatibility as much as I can.

Hm. So I sort of accomplished what I was hoping for above. I moved two functions to a utils file (.js, not .jsx), and it works. However, now I've run into another slight issue. I need to get the question type from the backend - and I forgot about this in the moment of adding the endpoint. I'll see what I can do about this...

Alright, I found a way to include the question type in the `GetQuestionById` endpoint, and it wasn't too much of a workaround. Had to use `.Where` instead of `.FindAsync`, which is a little sad, but it's fine. It works now, and it did lead me to discover a flaw in one of the unit tests for the related service class method. Namely, I'd forgotten about `await _context.SaveChangesAsync();`. The test passed before I made the changes related to also getting the question type, but failed after I applied them. This is because I had forgotten about the above. So, the question that was added to the context in the test wasn't *fully* added when I ran it, rather, it was sort of saved in a half-complete state. Thus, the discriminator column value ('QuestionType') has yet to receive a value, and so the test failed because it found no 'QuestionType' value for the requested question. I can imagine I'd spend a lot of time debugging that if I hadn't noticed my mistake...

Also, sidenote, I feel inclined to document here that I learned about an interesting status code. 418 I'm a teapot. It has nothing to do with my API, and I won't use it, but I found it to be rather hilarious nontheless. Apparently it's related to an April Fools' joke from 1998. Anyhow, I have a `ModifyQuestion` component to set up, gotta go fix that...

Right, the component is now up and fully functional (I think?). All seems to work just as I'd hoped. First, you enter an ID (1 or above), and click 'Get Question'. That question is fetched from the SQLite database, and all its content is displayed in the HTML form below. Then, you can freely change anything in that form (except for the question type - that one is locked), and click 'Update Question'. 

While writing the above, I realized something. What if you delete the ID before clicking 'Update Question'? Answer: The request fails. Obviously. So, I fixed that by slapping some extra `!(questionId > 0)` checks at the places in the code where I enable/disable the button in question. And now it works, so I think that's the `ModifyQuestion` component done? All that is left to do now is to add the DELETE endpoint and potentially add some extra CSS to make things look even more spicy.

2024-05-13
-----------
Another quick backend detour later, and I've now refactored the method that deserializes the JSON related to creating a question. Previously, I had a switch statement for every question type (10 cases, 11 counting the default for invalid question type). Not very good. So, today I learned about `Type type = Type.GetType(string className)`. I feel like I recall doing something similar, long ago... Anyhow, this I wanted to use to translate the string `questionType` into a question data type to use for deserialization. Eleminates the need for the switch statement. I struggled a little bit with this, because `GetType` was returning null. I figured it would be a somewhat finicky method to use, and I wasn't completely wrong. It wasn't *quite* as straightforward as one may or may not expect. Anyway, directly supplying the class name like this: 

```cs
Type type = Type.GetType(questionType)!;
```

Did not work. `type` was null. Instead, I provided the path to the correct class, like this:

```cs
Type type = Type.GetType("Backend.Infrastructure.Models.Entities.QuestionTypes." + questionType)!;
```

And just like that, it worked. I could then use `type` like this:

```cs
FourOptionQuestion? question = (FourOptionQuestion?)JsonSerializer.Deserialize(fourOptionQuestionJson, type, _serializerOptions);
```

To get a question of the correct question type. Question is not allowed to be an instance of `FourOptionQuestion` - it's an abstract class. Instead, the question type has to be of a real question type, all of which inherit from `FourOptionQuestion`. I haven't actually tested what happens if the user decides it's time for a little trolling and supplies a question type of `FourOption`. Cause that will result in a question type `FourOptionQuestion`. Looks familar?

To answer the above question that likely no one but me asked: `Invalid question type. Please verify that you chose a valid question type.` When formatting `questionType` to fit a question type, I'm doing the following:

```cs
questionType = textInfo.ToTitleCase(questionType) + "Question";
```

This works well enough. A `questionType` of 'FourOption' becomes 'Fouroption'. So that's why `type` is null - 'FouroptionQuestion' doesn't match 'FourOptionQuestion' (notice the non-capitalized 'O' in 'Option', in the former). And, this means that this solution will *only* work for question types that are one word long. It won't work with a name like `VerySpecialQuestion`, so that's worth keeping note of. But it works for the classes I have, where the names are something like `GeographyQuestion`, `LiteratureQuestion`, etc. Also, for once, the tests didn't break. So that's nice, and as a bonus, frontend question creation still works. I'd almost be more impressed if it didn't, but eh, you never know.

2024-05-14
----------
I'm very confused. I removed a backend swagger-documentation-related NuGet package from the project and wanted to test that it still worked. So, I booted up the frontend and backend, took the quiz, requested 2 questions, and got 5. The database, however, said 2. I had received and answered five questions, 3 of which I got correct. So, my total number of questions in the database was 2. But I had 3 correct answers. I really have no clue what happened, what went wrong, or how to replicate it. Three attempts at replicating the issue failed at replicating the issue, because they all behaved as expected. Oh well, it seems to be working, so I'll just pretend like it never failed at all...

I've now made some minor CSS changes to the frontend. Biggest one was to center (centralize?) the buttons and instructions for the initial page component. 2/2 of my sources have suggested this change, and so I shall test it.

2024-05-15
-----------
Alright. The final option, to delete a question, has now been added and its functionality confirmed. I also tested the project as a whole by taking the quiz, adding questions, modifying questions, and then deleting them. The database seems to update accordingly each time, so I think it's almost done? I know its not done too nicely, some of the functions involved some copy-pasting, but it'll suffice. I don't feel like deep-diving into object oriented JavaScript, when the language is as unpredictable as it is. However! One feature I know I want to add is an option to return to the 'main menu'. Currently, The only way to navigate back up the chain of components is to reload the page. I'll see if I can go fix that...

Hmkay, a `Menu` button has now been added. Because of how I handle the currently loaded component, it was really easy to implement. What wasn't as easy was making the button look OK, while also having it be placed at a reasonable position. It's size should be responsive, it should be big enough to be easy to click, but small enough so that it isn't in the way. And it should also fit at the same position throughout all the components... I'm not sure if I succeeded in all those steps, but at least I think it looks decent enough.

I'm currently not sure what else to add to the quiz project. I think I'll add some more questions, and wait for potential input from new sources.

2024-05-16
-----------
So I noticed something curious regarding the menu button I added yesterday. Apparently, it was partially covered by a span in the `TakeQuiz` component. So, I slapped a `pointer-events: none;` on that span and boom. Easy fix. Other than that, I'm simply gathering postman requests for some more questions. I'd like 10 of each question type, and I'm starting to think I was a little unnecessarily ambitious with the amount of question types. I have ten of them. So that means 100 questions. That's going to take a while.

Some time later, I've now added a few more questions. I have a total of 90, now. Just ten more to go, in other words. That will be tomorrow, though.

2024-05-18
-----------
I've now created a few more questions. Not quite there yet, maybe later today?

Alright. One hundred questions manually added. To Postman. Gonna take a while to make those 100 HTTP POST requests, eh. I'll drop the database, rebuild it, add all the questions, and save the file. I am *not* recreating all the questions manually each time something goes sour with the database. At least I hope I won't have to - it's not like I can gurantee anything...

Right, this was a confusing little journey I just embarked on. So, after adding all the questions, I wanted to back up the database. I noticed that a .db-wal file contained all the questions, instead of them being saved to the main database. Seems to be some middle-step, perhaps to make sure that nothing goes wrong while adding stuff to the database. So I wasn't sure how to back up the database, since now more files than just the .db file existed. I thought to use the sqlite3 shell to force SQLite to merge the files together, effectively adding all the data to the main .db file. But when I finally had everything set up, the merge had happened automatically. I can't exaclty complain, since it seems to be working, but still. It could have been a tad more smooth...

Hm. The database should be backed up and safe now. I think. I'll test the quiz for a bit, and see if all works well.

Hm. I noticed something. When I added the `Menu` button, that pushed down the `flash` div, meaning that no longer does the entire background flash green/red depending on the answer. Well, now it does, because I fixed it. But I had to fix it because it broke. So I added another div and slapped some spaghetti CSS on that. Not very clean, but that's what tends to happen when I try to write CSS. At least it seems to work, I think?