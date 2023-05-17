using CommBank.Models;
using CommBank.Services;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Secrets.json");

var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("CommBank"));
var mongoDatabase = mongoClient.GetDatabase("CommBank");

var collections = await mongoDatabase.ListCollectionsAsync();

if (collections.Any() == false)
{
    var collectionUsers = mongoDatabase.GetCollection<User>("Users");
    string textUsers = File.ReadAllText(@"Data/Users.json");
    var users = BsonSerializer.Deserialize<List<User>>(textUsers);
    await collectionUsers.InsertManyAsync(users);

    var collectionTransactions = mongoDatabase.GetCollection<CommBank.Models.Transaction>("Transactions");
    string textTransactions = File.ReadAllText(@"Data/Transactions.json");
    var transactions = BsonSerializer.Deserialize<List<CommBank.Models.Transaction>>(textTransactions);
    await collectionTransactions.InsertManyAsync(transactions);

    var collectionTags = mongoDatabase.GetCollection<CommBank.Models.Tag>("Tags");
    string textTags = File.ReadAllText(@"Data/Tags.json");
    var tags = BsonSerializer.Deserialize<List<CommBank.Models.Tag>>(textTags);
    await collectionTags.InsertManyAsync(tags);

    var collectionGoals = mongoDatabase.GetCollection<Goal>("Goals");
    string textGoals = File.ReadAllText(@"Data/Goals.json");
    var goals = BsonSerializer.Deserialize<List<Goal>>(textGoals);
    await collectionGoals.InsertManyAsync(goals);

    var collectionAccounts = mongoDatabase.GetCollection<Account>("Accounts");
    string textAccounts = File.ReadAllText(@"Data/Accounts.json");
    var accounts = BsonSerializer.Deserialize<List<Account>>(textAccounts);
    await collectionAccounts.InsertManyAsync(accounts);
}



IAccountsService accountsService = new AccountsService(mongoDatabase);
IAuthService authService = new AuthService(mongoDatabase);
IGoalsService goalsService = new GoalsService(mongoDatabase);
ITagsService tagsService = new TagsService(mongoDatabase);
ITransactionsService transactionsService = new TransactionsService(mongoDatabase);
IUsersService usersService = new UsersService(mongoDatabase);

builder.Services.AddSingleton(accountsService);
builder.Services.AddSingleton(authService);
builder.Services.AddSingleton(goalsService);
builder.Services.AddSingleton(tagsService);
builder.Services.AddSingleton(transactionsService);
builder.Services.AddSingleton(usersService);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
   .AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

