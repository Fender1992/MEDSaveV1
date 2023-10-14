using System.Text;
using System.Text.Json;
using System.Net;
using Amazon.Runtime;
using Amazon.CognitoIdentityProvider;
using Amazon;
using Amazon.CognitoIdentityProvider.Model;
using MEDSaveV1.Views;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;

namespace MEDSaveV1
{
    public partial class MainPage : ContentPage
    {
        static async Task<Dictionary<string, string>> GetSecret()
        {
            string secretName = "dev/rolo";
            string region = "us-east-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT",
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                throw e;
            }

            return JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);
        }

        public MainPage()
        {
            InitializeComponent();
        }

        async void LoginClick(object sender, EventArgs e)
        {
            var secrets = await GetSecret();
            string AWS_ACCESS_KEY = secrets["AccessKey"];
            string AWS_SECRET_KEY = secrets["SecretKey"];

            if (string.IsNullOrWhiteSpace(Email.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                await DisplayAlert("Error", "Both email and password are required.", "OK");
                return;
            }

            var awsCredentials = new BasicAWSCredentials(AWS_ACCESS_KEY, AWS_SECRET_KEY);
            var cognitoProvider = new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.USEast1);

            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = "776tr7h43a3d3js1p3au4e2pse",
                AuthParameters = new Dictionary<string, string>
                {
                    {"USERNAME", Email.Text},
                    {"PASSWORD", Password.Text}
                }
            };

            string authToken = "Authorization";

            try
            {
                var authResponse = await cognitoProvider.InitiateAuthAsync(authRequest);

                if (authResponse.AuthenticationResult != null)
                {
                    authToken = authResponse.AuthenticationResult.IdToken;
                }
                else
                {
                    // Handle authentication error
                    await DisplayAlert("Error", "Authentication failed.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Cognito Exception: {ex.Message}", "OK");
                return;
            }

            var userCredentials = new
            {
                Email = Email.Text,
                Password = Password.Text
            };

            var json = JsonSerializer.Serialize(userCredentials);
            var apiUrl = "https://p10c8i9bqb.execute-api.us-east-1.amazonaws.com/test/users/login";

            using (HttpClient client = new HttpClient())
            {
                // Attach the token to the HttpClient request
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                try
                {
                    var response = await client.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Logged in successfully.", "OK");
                        await Navigation.PushAsync(new ServicesPage());
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        await DisplayAlert("Error", "User does not exist.", "OK");
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Error", $"Failed to log in. Response: {responseBody}", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
                }
            }
        }

        private void GoogleLoginClick(object sender, EventArgs e)
        {
            // Logic to initiate Google federated sign-in
        }

        private async void HospitalClick(object sender, EventArgs e)
        {
            //calls just one facility
            //var apiUrl = "https://l4h2elr2e4.execute-api.us-east-1.amazonaws.com/Test/facility?facilityId=324285b4-ebd5-401c-a312-e3b960f6596c";
            //trying to use this one to call the whole table
            //var apiUrl2 = "https://l4h2elr2e4.execute-api.us-east-1.amazonaws.com/Test/facility?facilityId=7e5e2d94-c145-468e-a9b3-e55bb66b8fa1";

            string[] apiStrings = new string[2];
            apiStrings[0] = "https://l4h2elr2e4.execute-api.us-east-1.amazonaws.com/Test/facility?facilityId=324285b4-ebd5-401c-a312-e3b960f6596c";
            apiStrings[1] = "https://l4h2elr2e4.execute-api.us-east-1.amazonaws.com/Test/facility?facilityId=7e5e2d94-c145-468e-a9b3-e55bb66b8fa1";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    for (var i = 0; i < apiStrings.Length; i++)
                    {
                        var response = await client.GetAsync(apiStrings[i]);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"API Response for URL {i}:");
                            Console.WriteLine(responseBody);
                        }
                        else
                        {
                            Console.WriteLine($"Error calling API for URL {i}. Status code: {response.StatusCode}. Response: {responseBody}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }


        protected void CreateUserClick(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateUserPage());
        }


    }
}