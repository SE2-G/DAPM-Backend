﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.Models
{
    public class FileDTO
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }

    }
}