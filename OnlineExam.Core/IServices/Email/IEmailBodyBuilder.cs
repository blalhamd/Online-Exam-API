﻿namespace OnlineExam.Core.IServices.Email
{
    public interface IEmailBodyBuilder
    {
        Task<string> GenerateEmailBody(string templateName, string imageUrl, string header, string TextBody, string link, string linkTitle);
    }
}
