﻿// -----------------------------------------------------------------------
// <copyright file="RequestException.cs" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sitecore.Performance.LhDataGenerator
{
    using System;

    /// <summary>
    /// Class RequestException.
    /// </summary>
    public class RequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class.
        /// </summary>
        public RequestException()
        {
            this.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public RequestException(Exception ex)
            : this()
        {
            this.ExceptionType = ex.GetType().Name;
            this.StackTrace = ex.StackTrace;
            this.Message = ex.Message;

            if (ex.InnerException != null)
            {
                this.InnerException = new RequestException(ex.InnerException);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public RequestException(RequestException exception)
        {
            this.ExceptionType = exception.ExceptionType;
            this.TimeStamp = new DateTime(exception.TimeStamp.Ticks);
            this.Message = exception.Message;
            this.StackTrace = exception.StackTrace;

            if (exception.InnerException != null)
            {
                this.InnerException = new RequestException(exception.InnerException);
            }
        }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>The type of the exception.</value>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>The inner exception.</value>
        public RequestException InnerException { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.Message + this.StackTrace;
        }
    }
}
