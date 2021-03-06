﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Properties;
using System.Web.Http.OData.Query.Validators;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// This defines a $orderby OData query option that can be used to perform query composition. 
    /// </summary>
    public class OrderByQueryOption
    {
        private OrderByClause _orderByClause;
        private ICollection<OrderByPropertyNode> _propertyNodes;
        private OrderByQueryValidator _validator;

        /// <summary>
        /// Initialize a new instance of <see cref="OrderByQueryOption"/> based on the raw $orderby value and 
        /// an EdmModel from <see cref="ODataQueryContext"/>.
        /// </summary>
        /// <param name="rawValue">The raw value for $orderby query. It can be null or empty.</param>
        /// <param name="context">The <see cref="ODataQueryContext"/> which contains the <see cref="IEdmModel"/> and some type information</param>
        public OrderByQueryOption(string rawValue, ODataQueryContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull("context");
            }

            if (String.IsNullOrEmpty(rawValue))
            {
                throw Error.ArgumentNullOrEmpty("rawValue");
            }

            Context = context;
            RawValue = rawValue;
            Validator = new OrderByQueryValidator();
        }

        /// <summary>
        ///  Gets the given <see cref="ODataQueryContext"/>.
        /// </summary>
        public ODataQueryContext Context { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="OrderByPropertyNode"/> instance
        /// for the current <see cref="OrderByQueryOption"/>.
        /// </summary>
        /// <remarks>
        /// This collection can be modified as needed.
        /// </remarks>
        public ICollection<OrderByPropertyNode> PropertyNodes
        {
            get
            {
                if (_propertyNodes == null)
                {
                    _propertyNodes = OrderByPropertyNode.CreateCollection(OrderByClause);
                }
                return _propertyNodes;
            }
        }

        /// <summary>
        ///  Gets the raw $orderby value.
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Gets or sets the OrderBy Query Validator
        /// </summary>
        public OrderByQueryValidator Validator
        {
            get
            {
                return _validator;
            }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _validator = value;
            }
        }

        /// <summary>
        /// Gets the parsed <see cref="OrderByClause"/> for this query option.
        /// </summary>
        private OrderByClause OrderByClause
        {
            get
            {
                if (_orderByClause == null)
                {
                    _orderByClause = ODataUriParser.ParseOrderBy(RawValue, Context.Model, Context.EntitySet.ElementType);
                }
                return _orderByClause;
            }
        }

        /// <summary>
        /// Apply the $orderby query to the given IQueryable.
        /// </summary>
        /// <param name="query">The IQueryable that we are applying orderby query against.</param>
        /// <returns>The query that the orderby query has been applied to.</returns>
        public IOrderedQueryable<T> ApplyTo<T>(IQueryable<T> query)
        {
            return ApplyToCore(query) as IOrderedQueryable<T>;
        }

        /// <summary>
        /// Apply the $orderby query to the given IQueryable.
        /// </summary>
        /// <param name="query">The IQueryable that we are applying orderby query against.</param>
        /// <returns>The query that the orderby query has been applied to.</returns>
        public IOrderedQueryable ApplyTo(IQueryable query)
        {
            return ApplyToCore(query);
        }

        public void Validate(ODataValidationSettings validationSettings)
        {
            if (validationSettings == null)
            {
                throw Error.ArgumentNull("validationSettings");
            }

            Validator.Validate(this, validationSettings);
        }

        private IOrderedQueryable ApplyToCore(IQueryable query)
        {
            // TODO 463999: [OData] Consider moving OrderByPropertyNode to ODataLib
            ICollection<OrderByPropertyNode> props = PropertyNodes;

            bool alreadyOrdered = false;
            IQueryable querySoFar = query;
            HashSet<IEdmProperty> propertiesSoFar = new HashSet<IEdmProperty>();

            foreach (OrderByPropertyNode prop in props)
            {
                IEdmProperty property = prop.Property;
                OrderByDirection direction = prop.Direction;

                // This check prevents queries with duplicate properties (e.g. $orderby=Id,Id,Id,Id...) from causing stack overflows
                if (propertiesSoFar.Contains(property))
                {
                    throw new ODataException(Error.Format(SRResources.OrderByDuplicateProperty, property.Name));
                }
                propertiesSoFar.Add(property);

                querySoFar = ExpressionHelpers.OrderBy(querySoFar, property, direction, Context.EntityClrType, alreadyOrdered);
                alreadyOrdered = true;
            }

            return querySoFar as IOrderedQueryable;
        }
    }
}
