using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplyLearn
{
	
	public class Trainer
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }

		/// <summary>
		/// Register a Trainer
		/// </summary>
		/// <returns>trainerID</returns>
		public RegisterResponse Register(IRepository repository)
		{
			
			int? trainerId = null;
			bool good = false;
			bool appr = false;
			
			var ot = new List<string>() { "vb6", "assembly", "forrtan", "VBScript" };

			//DEFECT #5274 DA 12/10/2012
			//We weren't filtering out the domain so I added it. 
			var domains = new List<string>() { "gmail.com", "yahoo.com", "hotmail.com" };

			if (!string.IsNullOrWhiteSpace(FirstName))
			{
				if (!string.IsNullOrWhiteSpace(LastName))
				{
					if (!string.IsNullOrWhiteSpace(Email))
					{
						//put list of employers in array
						var emps = new List<string>() { "Salesforce", "Microsoft", "Google", "Amazon" };

						good = Exp > 10 || HasBlog || Certifications.Count() > 3 || emps.Contains(Employer);

						if (!good)
						{
							//need to get just the domain from the email
							string emailDomain = Email.Split('@').Last();

							if (!domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
							{
								good = true;
							}
						}

						if (good)
						{
							if (Sessions.Count() != 0)
							{
								foreach (var session in Sessions)
								{
									//foreach (var tech in nt)
									//{
									//    if (session.Title.Contains(tech))
									//    {
									//        session.Approved = true;
									//        break;
									//    }
									//}

									foreach (var tech in ot)
									{
										if (session.Title.Contains(tech) || session.Description.Contains(tech))
										{
											session.Approved = false;
											break;
										}
										else
										{
											session.Approved = true;
											appr = true;
										}
									}
								}
							}
							else
							{
								return new RegisterResponse(RegisterError.NoSessionsProvided);
							}

							if (appr)
							{
								//if we got this far, the speaker is approved
								//let's go ahead and register him/her now.
								//First, let's calculate the registration fee. 
								//More experienced speakers pay a lower fee.								
								if (Exp <= 1)
								{
									RegistrationFee = 500;
								}
								else if (Exp >= 2 && Exp <= 3)
								{
									RegistrationFee = 250;
								}
								else if (Exp >= 4 && Exp <= 5)
								{
									RegistrationFee = 100;
								}
								else if (Exp >= 6 && Exp <= 9)
								{
									RegistrationFee = 50;
								}
								else
								{
									RegistrationFee = 0;
								}

							//Now, save the speaker and sessions to the db.
								try
								{
									trainerId = repository.SaveTrainer(this);
								}
								catch (Exception e)
								{
									//in case the db call fails 
								}
							}
							else
							{
								return new RegisterResponse(RegisterError.NoSessionsApproved);
							}
						}
						else
						{
							return new RegisterResponse(RegisterError.TrainerDoesNotMeetStandards);
						}
					}
					else
					{
						return new RegisterResponse(RegisterError.EmailRequired);
					}
				}
				else
				{
					return new RegisterResponse(RegisterError.LastNameRequired);
				}
			}
			else
			{
				return new RegisterResponse(RegisterError.FirstNameRequired);
			}

			//if we got this far, the speaker is registered.
			return new RegisterResponse((int)trainerId);
		}
	}
}