using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace user_service_lambda
{
    public class Function
    {
        private DynamoDBContext _dynamoDbContext;

        public Function()
        {
            _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        }

        public Dictionary<string, object> PreSignUpFunctionHandler(Dictionary<string, object> request, ILambdaContext context)
        {
            request["response"] = new Dictionary<string, object>
            {
                { "autoConfirmUser", true }
            };
            return request;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Inside FunctionHandler...");

            if (request.Resource == "/pre-sign-up" && request.HttpMethod == "POST")
            {
                LambdaLogger.Log("Processing pre-sign-up...");

                var requestBody = JsonSerializer.Deserialize<Dictionary<string, object>>(request.Body);

                var preSignUpResponse = PreSignUpFunctionHandler(requestBody, context);

                LambdaLogger.Log("Finished pre-sign-up processing...");

                return new APIGatewayProxyResponse()
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(preSignUpResponse)
                };
            }

            if (request.Resource == "/login" && request.HttpMethod == "POST")
            {
                return await HandleLogin(request);
            }

            return request.HttpMethod switch
            {
                "GET" => HandleGet(request),
                "POST" => await HandlePost(request),
                _ => new APIGatewayProxyResponse()
                {
                    StatusCode = 500,
                    Body = "Unknown Request"
                }
            };
        }


        private async Task<APIGatewayProxyResponse> HandlePost(APIGatewayProxyRequest request)
        {
            var user = JsonSerializer.Deserialize<User>(request.Body);
            if (user != null)
            {
                await _dynamoDbContext.SaveAsync(user);
                return new APIGatewayProxyResponse()
                {
                    StatusCode = 200,
                    Body = "User Added"
                };
            }
            return new APIGatewayProxyResponse()
            {
                StatusCode = 400,
                Body = "Bad Request"
            };
        }

        private APIGatewayProxyResponse HandleGet(APIGatewayProxyRequest request)
        {
            var userIdString = request.QueryStringParameters?["userId"];
            if (Guid.TryParse(userIdString, out var userId))
            {
                var user = _dynamoDbContext.LoadAsync<User>(userId);
                if (user != null)
                    return new APIGatewayProxyResponse()
                    {
                        StatusCode = 200,
                        Body = JsonSerializer.Serialize(user)
                    };
            }

            return new APIGatewayProxyResponse()
            {
                StatusCode = 400,
                Body = "Bad Request"
            };
        }
        


        private async Task<APIGatewayProxyResponse> HandleLogin(APIGatewayProxyRequest request)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(request.Body);
            if (loginRequest != null)
            {
                var existingUser = await _dynamoDbContext.LoadAsync<User>(loginRequest.Email);
                if (existingUser != null && existingUser.Password == loginRequest.Password) // Password checking for simplicity
                {
                    return new APIGatewayProxyResponse()
                    {
                        StatusCode = 200,
                        Body = "Login Successful"
                    };
                }
                return new APIGatewayProxyResponse()
                {
                    StatusCode = 401,
                    Body = "Invalid Credentials"
                };
            }
            return new APIGatewayProxyResponse()
            {
                StatusCode = 400,
                Body = "Bad Request"
            };
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class User
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    [DynamoDBHashKey]
    public Guid Id { get; set; }
}