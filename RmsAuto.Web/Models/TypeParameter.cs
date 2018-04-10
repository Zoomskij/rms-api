﻿using System.ComponentModel.DataAnnotations;

public enum TypeParameter
{
    [Display(Name = "Path")]
    path = 1,
    [Display(Name = "Query")]
    query = 2,
    [Display(Name = "Body")]
    body = 3
}