using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace facility_lambda;

public class Function
{
    public async Task<APIGatewayProxyResponse> FunctionHandler(
    APIGatewayProxyRequest request, ILambdaContext context)
    {
        var facilityIdString = request.QueryStringParameters?["facilityId"];
        if (Guid.TryParse(facilityIdString, out var facilityId))
        {
            var dynamoDbContext = new Amazon.DynamoDBv2.DataModel.DynamoDBContext(new AmazonDynamoDBClient());
            var facility = await dynamoDbContext.LoadAsync<Facility>(facilityId);
            if (facility != null)
                return new APIGatewayProxyResponse()
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(facility)
                };
        }

        return new APIGatewayProxyResponse()
        {
            StatusCode = 400,
            Body = "Bad Request"
        };
    }

}

public class Facility
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public HashSet<string> Services { get; set; }

}

