﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Web.Http.OData.Properties;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// Class describing the <see cref="IEdmProperty"/> and <see cref="OrderByDirection"/> for a single property
    /// in an OrderBy expression.
    /// </summary>
    public class OrderByPropertyNode
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="OrderByPropertyNode"/> class.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> for this node.</param>
        /// <param name="direction">The <see cref="OrderByDirection"/> for this node.</param>
        public OrderByPropertyNode(IEdmProperty property, OrderByDirection direction)
        {
            if (property == null)
            {
                throw Error.ArgumentNull("property");
            }

            Property = property;
            Direction = direction;
        }

        /// <summary>
        /// Gets the <see cref="IEdmProperty"/> for the current node.
        /// </summary>
        public IEdmProperty Property { get; private set; }

        /// <summary>
        /// Gets the <see cref="OrderByDirection"/> for the current node.
        /// </summary>
        public OrderByDirection Direction { get; private set; }

        /// <summary>
        /// Creates a collection of <see cref="OrderByPropertyNode"/> instances from a linked list of <see cref="OrderByClause"/> instances.
        /// </summary>
        /// <param name="orderByClause">The head of the <see cref="OrderByClause"/> linked list.</param>
        /// <returns>The collection of new <see cref="OrderByPropertyNode"/> instances.</returns>
        public static ICollection<OrderByPropertyNode> CreateCollection(OrderByClause orderByClause)
        {
            LinkedList<OrderByPropertyNode> result = new LinkedList<OrderByPropertyNode>();
            for (OrderByClause clause = orderByClause; clause != null; clause = clause.ThenBy)
            {
                SingleValuePropertyAccessNode property = clause.Expression as SingleValuePropertyAccessNode;

                if (property == null)
                {
                    throw new ODataException(SRResources.OrderByPropertyNotFound);
                }
                result.AddLast(new OrderByPropertyNode(property.Property, clause.Direction));
            }

            return result;
        }
    }
}
