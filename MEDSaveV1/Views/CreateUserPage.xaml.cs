namespace MEDSaveV1.Views;
using MEDSaveV1.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
public partial class CreateUserPage : ContentPage
{
    public CreateUserPage()
    {
        InitializeComponent();
    }

    async void CreateUserClick(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserName.Text) ||
            string.IsNullOrWhiteSpace(FirstName.Text) ||
            string.IsNullOrWhiteSpace(LastName.Text) ||
            string.IsNullOrWhiteSpace(Email.Text) ||
            string.IsNullOrWhiteSpace(Password.Text))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }

        var newUser = new User
        {
            Username = UserName.Text,
            Firstname = FirstName.Text,
            Lastname = LastName.Text,
            Email = Email.Text,
            Password = Password.Text,
            Id = Guid.NewGuid()
        };

        // Serialize newUser object
        var json = JsonSerializer.Serialize(newUser);

        // Assuming your Lambda function is exposed via an API Gateway endpoint
        //var apiUrl = "https://p10c8i9bqb.execute-api.us-east-1.amazonaws.com/Dev/users";
        var apiUrl = "https://p10c8i9bqb.execute-api.us-east-1.amazonaws.com/test/users";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    await DisplayAlert("Success", "User created successfully.", "OK");
                    await Navigation.PushAsync(new ServicesPage());
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"There was an error creating the user. Response: {responseBody}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
            }
        }


    }
}