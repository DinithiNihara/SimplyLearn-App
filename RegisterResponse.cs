namespace SimplyLearn
{
	public class RegisterResponse
	{
		public RegisterResponse(int trainerId)
		{
			this.TrainerId = trainerId;
		}

		public RegisterResponse(RegisterError? error)
		{
			this.Error = error;
		}

		public int? TrainerId { get; set; }
		public RegisterError? Error { get; set; }
	}

	public enum RegisterError
	{
		FirstNameRequired,
		LastNameRequired,
		EmailRequired,
		NoSessionsProvided,
		NoSessionsApproved,
		TrainerDoesNotMeetStandards
	};
}
