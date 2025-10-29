//using Application.Objects.Auth;
//using Application.Services.Auth.Interfaces;
//using Application.Services.Database.Interfaces;
//using Models.Models;

//namespace Application.Services.Auth.Implementations;
//public class AuthService(
//    IUserService userService,
//    JwtTokenGenerator jwtTokenGenerator,
//    RefreshTokenService refreshTokenGenerator,
//) {
//    private async Task<AuthResult> GenerateTokensAsync(User user, int expiresInSeconds, CancellationToken ct) {
//        JwtUserClaims jwtUser = new() {
//            PublicId = user.Id.ToString(),
//            Email = user.Email,
//        };

//        string jwtToken = jwtTokenGenerator.GetJwtToken(jwtUser, expiresInSeconds);
//        string refreshToken = refreshTokenGenerator.GenerateRefreshToken();
//        await refreshTokenGenerator.SaveRefreshToken(refreshToken, user, ct);

//        return new AuthResult(jwtToken, refreshToken);
//    }

//    private async Task<AuthResult> LoginWithOAuthAsync(OAuthUser authUser, CancellationToken ct) {
//        User user = await userService.FindOrCreateAuthenticatedUserAsync(authUser, ct);

//        return await GenerateTokensAsync(user, 300, ct);
//    }

//    public async Task<AuthResult> LoginWithGoogleAsync(string token, CancellationToken ct)
//        => await LoginWithOAuthAsync(await providers.ValidateGoogleTokenAsync(token, ct), ct);

//    public async Task<AuthResult> LoginWithMicrosoftAsync(string token, CancellationToken ct)
//        => await LoginWithOAuthAsync(await providers.ValidateMicrosoftTokenAsync(token, ct), ct);

//    public async Task<AuthResult> LoginWithGitHubAsync(string token, CancellationToken ct)
//        => await LoginWithOAuthAsync(await providers.ValidateGitHubCodeAsync(token, ct), ct);

//    public async Task<AuthResult> LoginWithGitLabAsync(string token, CancellationToken ct)
//        => await LoginWithOAuthAsync(await providers.ValidateGitLabCodeAsync(token, ct), ct);


//    /// <summary>
//    /// Authenticates a user using the provided login credentials and generates authentication tokens.
//    /// </summary>
//    /// <remarks>This method verifies the user's credentials and generates a set of authentication tokens 
//    /// with a default expiration time of 300 seconds. Ensure that the <paramref name="ct"/>  is properly
//    /// handled to avoid unnecessary processing.</remarks>
//    /// <param name="login">The login identifier of the user. This cannot be null or empty.</param>
//    /// <param name="password">The password associated with the user's account. This cannot be null or empty.</param>
//    /// <param name="ct">A token to monitor for cancellation requests.</param>
//    /// <returns>An <see cref="AuthResult"/> containing the generated authentication tokens for the authenticated user.</returns>
//    public async Task<AuthResult?> LoginManuallyAsync(string login, string password, CancellationToken ct) {
//        User? user = await userService.GetByLoginAsync(login, ct);
//        if (user is null) {
//            return null;
//        }
//        var passwordData = await userService.GetPasswordData(user.Id, ct);
//        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, passwordData?.HashedPassword);
//        return isPasswordValid ? await GenerateTokensAsync(user, 300, ct) : null;
//    }


//    /// <summary>
//    /// Registers a new user with the provided authentication details.
//    /// </summary>
//    /// <remarks>This method checks if a user with the specified email already exists. If a user is found, an
//    /// <see cref="InvalidOperationException"/> is thrown. The user's password is securely hashed before being
//    /// stored.</remarks>
//    /// <param name="authUser">The authentication details of the user to be registered, including email, password, and optional personal
//    /// information.</param>
//    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
//    /// <returns>A <see cref="User"/> object representing the newly created user.</returns>
//    /// <exception cref="InvalidOperationException">Thrown if a user with the specified email already exists.</exception>
//    public async Task<User> SignupUserAsync(ManualAuthUser authUser, CancellationToken ct) {
//        // Check if user exists
//        var existingUser = await userService.GetByLoginAsync(authUser.Email, ct);
//        if (existingUser != null) {
//            throw new InvalidOperationException("User already exists");
//        }

//        // Create user
//        var user = new User {
//            Username = authUser.Username,
//            FirstName = authUser.FirstName,
//            MiddleName = authUser.MiddleName,
//            LastName = authUser.LastName,
//            Email = authUser.Email,
//            JoinedAt = DateTime.UtcNow,
//            IsDeleted = false,
//            PasswordUserLogin = new PasswordUserLogin() {
//                HashedPassword = PasswordService.HashPassword(authUser.Password)
//            }
//        };

//        // Persist
//        await userService.CreateAsync(user, ct);
//        return user;
//    }


//    public async Task<AuthResult?> RefreshTokenAsync(string refreshToken, CancellationToken ct) {
//        var refreshTokenObj = await refreshTokenGenerator.GetTokenAsync(refreshToken, ct);
//        if (refreshTokenObj is null) {
//            return null;
//        }
//        await refreshTokenGenerator.RevokeToken(refreshToken, ct);

//        var user = await userService.GetAsync(refreshTokenObj.UserId, ct);
//        var tokens = await GenerateTokensAsync(user!, 300, ct);

//        return tokens;
//    }
//}
