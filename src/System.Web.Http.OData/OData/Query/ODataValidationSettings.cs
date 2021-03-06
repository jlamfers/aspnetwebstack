﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.ObjectModel;

namespace System.Web.Http.OData.Query
{
    public class ODataValidationSettings
    {
        private const int MinMaxSkip = 0;
        private const int MinMaxTop = 0;

        private AllowedArithmeticOperators _allowedArithmeticOperators;
        private AllowedFunctionNames _allowedFunctionNames;
        private AllowedLogicalOperators _allowedLogicalOperators;
        private AllowedQueryOptions _allowedQueryParameters;
        private Collection<string> _allowedOrderByProperties;
        private int? _maxSkip;
        private int? _maxTop;

        public ODataValidationSettings()
        {
            // default it to all the operators
            _allowedArithmeticOperators = AllowedArithmeticOperators.All;
            _allowedFunctionNames = AllowedFunctionNames.AllFunctionNames;
            _allowedLogicalOperators = AllowedLogicalOperators.All;
            _allowedQueryParameters = AllowedQueryOptions.All;
            _allowedOrderByProperties = new Collection<string>();
        }

        /// <summary>
        /// Gets/Sets a list of allowed arithmetic operators including 'add', 'sub', 'mul', 'div', 'mod'
        /// </summary>
        public AllowedArithmeticOperators AllowedArithmeticOperators
        {
            get
            {
                return _allowedArithmeticOperators;
            }
            set
            {
                if (value > AllowedArithmeticOperators.All || value < AllowedArithmeticOperators.None)
                {
                    throw Error.InvalidEnumArgument("value", (Int32)value, typeof(AllowedArithmeticOperators));
                }

                _allowedArithmeticOperators = value;
            }
        }

        /// <summary>
        /// Returns a list of allowed functions as follows.
        /// 
        /// Against edm.string (11)
        ///  substringof, endswith, startswith, length, indexof, replace, substring, tolower, toupper, trim, concat
        ///
        /// e.g. ~/Customers?$filter=length(CompanyName) eq 19
        ///
        /// Against edm.DateTime/DateTimeOffset (12)
        ///  year, years, month, months, day, days, hour, hours, minute, minutes, second, seconds
        ///
        /// e.g. ~/Employees?$filter=year(BirthDate) eq 1971
        ///
        /// Math related (3) 
        ///  round, floor, ceiling
        ///
        /// Against Type (2)
        ///  isof, cast, 
        ///
        /// Against Collection (2)
        ///  any, all
        ///  
        /// </summary>
        public AllowedFunctionNames AllowedFunctionNames
        {
            get
            {
                return _allowedFunctionNames;
            }
            set
            {
                if (value > AllowedFunctionNames.All || value < AllowedFunctionNames.None)
                {
                    throw Error.InvalidEnumArgument("value", (Int32)value, typeof(AllowedFunctionNames));
                }

                _allowedFunctionNames = value;
            }
        }

        /// <summary>
        /// Returns a list of allowed logical Operators such as 'eq', 'ne', 'gt', 'ge', 'lt', 'le', 'and', 'or', 'not'.
        /// </summary>
        public AllowedLogicalOperators AllowedLogicalOperators
        {
            get
            {
                return _allowedLogicalOperators;
            }
            set
            {
                if (value > AllowedLogicalOperators.All || value < AllowedLogicalOperators.None)
                {
                    throw Error.InvalidEnumArgument("value", (Int32)value, typeof(AllowedLogicalOperators));
                }

                _allowedLogicalOperators = value;
            }
        }

        /// <summary>
        /// Returns a list of properties one can orderby the result with. Note, by default if the list is empty, 
        /// it actually means it can be ordered by any properties.
        /// </summary>
        public Collection<string> AllowedOrderByProperties
        {
            get
            {
                return _allowedOrderByProperties;
            }
        }

        /// <summary>
        /// Returns the query parameters that you allowed, the default is all four query options, including $filter, $skip, $top, $orderby
        /// </summary>
        public AllowedQueryOptions AllowedQueryOptions
        {
            get
            {
                return _allowedQueryParameters;
            }
            set
            {
                if (value > AllowedQueryOptions.All || value < AllowedQueryOptions.None)
                {
                    throw Error.InvalidEnumArgument("value", (Int32)value, typeof(AllowedQueryOptions));
                }

                _allowedQueryParameters = value;
            }
        }

        /// <summary>
        /// Gets/Sets the max skip value that client can request
        /// </summary>
        public int? MaxSkip
        {
            get
            {
                return _maxSkip;
            }
            set
            {
                if (value.HasValue && value < MinMaxSkip)
                {
                    throw Error.ArgumentMustBeGreaterThanOrEqualTo("value", value, MinMaxSkip);
                }

                _maxSkip = value;
            }
        }

        /// <summary>
        /// Gets/Sets the max top value that client can request 
        /// </summary>
        public int? MaxTop
        {
            get
            {
                return _maxTop;
            }
            set
            {
                if (value.HasValue && value < MinMaxTop)
                {
                    throw Error.ArgumentMustBeGreaterThanOrEqualTo("value", value, MinMaxTop);
                }

                _maxTop = value;
            }
        }
    }
}
