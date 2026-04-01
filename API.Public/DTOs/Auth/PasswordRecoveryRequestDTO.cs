namespace API.Public.DTOs.Auth;

public record PasswordRecoveryRequestDTO(string Email);

public record PasswordRecoveryVerifyDTO(string Email, string Token);

public record PasswordResetDTO(string Email, string Token, string NewPassword);
