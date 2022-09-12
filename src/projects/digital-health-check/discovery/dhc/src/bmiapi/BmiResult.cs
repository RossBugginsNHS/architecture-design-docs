/*
 * Created Date: Wednesday, August 17th 2022, 09:23:49 am
 * Author: Ross Buggins (NHS) (78215796+RossBugginsNHS@users.noreply.github.com>)
 * -----
 * Last Modified: 12/09/2022 09:04:05 am
 * Modified By: Ross Buggins (NHS)
 * -----
 * Copyright (c) 2022 Crown Copyright
 * -----
 * GNU General Public License v3.0 or later
 * 
 * This file is part of NHS England Digital Health Check.
 * 
 * NHS England Digital Health Check is free software: you can redistribute it and/or modify it under the terms
 * of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License,
 * or (at your option) any later version.
 * 
 * NHS England Digital Health Check is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with NHS England Digital Health Check.
 * If not, see <https://www.gnu.org/licenses/>.
 * -----
 */

namespace bmiapi;

/// <summary>
/// A BMI result containing numerical and textual description values.
/// </summary>
public readonly record struct BmiResult
{
    public BmiResult(decimal bmi, string bmiDescription)
    {
        Bmi = bmi;
        BmiDescription = bmiDescription;
    }

    /// <summary>
    /// The bmi numerical value.
    /// </summary>
    /// <example>26</example>    
    public readonly decimal Bmi { get; init; }

    /// <summary>
    /// The bmi description.
    /// </summary>
    /// <example>Normal</example>    
    public readonly string BmiDescription { get; init; } 
}